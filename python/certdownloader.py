#!/usr/bin/env python3
"""Download all certificates in the chain for given HTTPS hosts.

Usage:
    python certdownloader.py <hostname> [<hostname> ...]

Each certificate from the retrieved chain is written to a PEM file named as
"<dns>_<common name>.pem" in the current working directory.
"""

import os
import sys
import socket
from urllib.parse import urlparse
from OpenSSL import SSL, crypto


def sanitize(text: str) -> str:
    """Return a filesystem-friendly version of *text*."""
    return "".join(c if c.isalnum() or c in "-_." else "_" for c in text)


def _create_socket(host: str, port: int) -> socket.socket:
    """Create a TCP connection, honouring HTTPS proxy settings if present."""
    proxy = os.environ.get("https_proxy") or os.environ.get("HTTPS_PROXY")
    if proxy:
        p = urlparse(proxy)
        sock = socket.create_connection((p.hostname, p.port))
        connect = f"CONNECT {host}:{port} HTTP/1.1\r\nHost: {host}:{port}\r\n\r\n"
        sock.sendall(connect.encode("ascii"))
        response = b""
        while b"\r\n\r\n" not in response:
            data = sock.recv(4096)
            if not data:
                break
            response += data
        status_line = response.split(b"\r\n", 1)[0]
        if b"200" not in status_line:
            raise OSError(f"Proxy CONNECT failed: {status_line.decode(errors='ignore')}")
        return sock
    return socket.create_connection((host, port))


def download_chain(url: str) -> None:
    parsed = urlparse(url if url.startswith("http") else f"https://{url}")
    host = parsed.hostname
    if not host:
        raise ValueError(f"Unable to parse host from URL: {url}")
    port = parsed.port or 443

    ctx = SSL.Context(SSL.TLS_CLIENT_METHOD)
    ctx.set_default_verify_paths()
    # Do not verify certificates – we only want to fetch them
    ctx.set_verify(SSL.VERIFY_NONE, lambda *args: True)

    sock = _create_socket(host, port)
    try:
        conn = SSL.Connection(ctx, sock)
        conn.set_tlsext_host_name(host.encode())
        conn.set_connect_state()
        conn.do_handshake()

        chain = conn.get_peer_cert_chain() or []
        if not chain:
            print(f"No certificates retrieved from {host}")
            return

        for cert in chain:
            cn = cert.get_subject().CN or "unknown"
            filename = f"{host}_{sanitize(cn)}.pem"
            with open(filename, "wb") as f:
                pem = crypto.dump_certificate(crypto.FILETYPE_PEM, cert)
                f.write(pem)
            print(f"Saved {filename}")
    finally:
        sock.close()


def main(argv: list[str]) -> int:
    if len(argv) < 2:
        print("Usage: python certdownloader.py <hostname> [<hostname> ...]")
        return 1
    for url in argv[1:]:
        download_chain(url)
    return 0


if __name__ == "__main__":
    raise SystemExit(main(sys.argv))
