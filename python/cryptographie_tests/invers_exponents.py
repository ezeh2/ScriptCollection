

#m1=9

#e=13
#d=37
#p=7
#q=11
#n=p*q


m1=9

e=17
d=33
p=5
q=11
n=p*q


c=pow(m1,e)%n
print(c)

m2=pow(c,d)%n
print(m2)

print("===")

m3=pow(m1,e*d) % n
print(m3)

m4=pow(m1,e*d) % p
print(m4)

m5=pow(m1,e*d) % q
print(m5)




