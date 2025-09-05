#!/usr/bin/env python3
"""Download certificate chains for HTTPS URLs.

Given one or more HTTPS URLs or hostnames, this script connects to each
and stores every certificate from the server's chain into individual PEM
files.  Each output file is named using the DNS name and the certificate's
common name (CN).
"""

import argparse
import os
import re
import socket
import urllib.parse

from OpenSSL import SSL, crypto


def _connect_via_proxy(host: str, port: int) -> socket.socket:
    """Establish a TCP connection to ``host`` using an HTTP proxy if configured."""
    proxy_url = os.environ.get("https_proxy") or os.environ.get("HTTPS_PROXY")
    if not proxy_url:
        return socket.create_connection((host, port))

    parsed = urllib.parse.urlparse(proxy_url)
    sock = socket.create_connection((parsed.hostname, parsed.port))
    connect_req = f"CONNECT {host}:{port} HTTP/1.1\r\nHost: {host}:{port}\r\n\r\n".encode()
    sock.sendall(connect_req)
    response = b""
    while b"\r\n\r\n" not in response:
        chunk = sock.recv(4096)
        if not chunk:
            break
        response += chunk
    if not response.startswith(b"HTTP/1.1 200"):
        raise RuntimeError(f"Proxy CONNECT failed: {response!r}")
    return sock


def download_cert_chain(url: str) -> None:
    parsed = urllib.parse.urlparse(url if "://" in url else f"https://{url}")
    host = parsed.hostname
    port = parsed.port or 443
    sock = _connect_via_proxy(host, port)

    ctx = SSL.Context(SSL.TLS_CLIENT_METHOD)
    ctx.set_default_verify_paths()
    conn = SSL.Connection(ctx, sock)
    conn.set_tlsext_host_name(host.encode())
    conn.set_connect_state()
    conn.do_handshake()

    chain = conn.get_peer_cert_chain() or []
    for idx, cert in enumerate(chain, 1):
        cn = cert.get_subject().CN or f"cert{idx}"
        cn_safe = re.sub(r"[^0-9A-Za-z_.-]", "_", cn)
        filename = f"{host}_{cn_safe}.pem"
        with open(filename, "wb") as f:
            f.write(crypto.dump_certificate(crypto.FILETYPE_PEM, cert))
        print(f"Saved {filename}")

    conn.close()
    sock.close()


def main() -> None:
    parser = argparse.ArgumentParser(description="Download certificate chains")
    parser.add_argument("urls", nargs="+", help="HTTPS URLs or hostnames")
    args = parser.parse_args()

    for url in args.urls:
        try:
            download_cert_chain(url)
        except Exception as exc:  # pragma: no cover - user feedback
            print(f"Failed to download certs for {url}: {exc}")


if __name__ == "__main__":
    main()
