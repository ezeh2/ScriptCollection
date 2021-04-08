
import dnslib
from dnslib.server import DNSServer, DNSLogger
import binascii
from dnslib.dns import RR

'''
packet = binascii.unhexlify(b'd5ad818000010005000000000377777706676f6f676c6503636f6d0000010001c00c0005000100000005000803777777016cc010c02c0001000100000005000442f95b68c02c0001000100000005000442f95b63c02c0001000100000005000442f95b67c02c0001000100000005000442f95b93')
d = dnslib.DNSRecord.parse(packet)
# print(d)

d2 = dnslib.DNSRecord.question("google.com")
print(d2)
'''

class TestResolver:
    def resolve(self,request,handler):
        print(request)
        reply=request.reply()

        reply.add_answer(*RR.fromZone("abc.def. 6000 A 1.2.3.4"))
        reply.add_answer(*RR.fromZone("blabla 6000 TXT 1.2.3.4"))
        return reply

resolver = TestResolver()
logger = DNSLogger(prefix=False)
server = DNSServer(resolver, port=8053, address="localhost", logger=logger, tcp=False)
server.start_thread()

