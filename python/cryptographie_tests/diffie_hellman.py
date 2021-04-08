

def get_factors():
    print("hello")

#OK:  2,13
#Bad: 3,13
#Bad: 4,13
#Bad: 5,13
#OK:  6,13
#OK:  7,13
#Bad:  8,13
#Bad:  9,13
#Bad:  10,13
#OK:   11,13
#Bad:   12,13
#factors_of_n_1=[2,3,4,6]         
    
#OK:   14,17
#factors_of_n_1=[2,4,8]

g=14
n=17
factors_of_n_1=[2,4,8]

print("===")
print("sollte 1 sein:")
print(pow(g,n-1)%n)
for f in factors_of_n_1:
    x=pow(g,f)%n
    print("sollte nicht 1 sein für Faktor "+str(f)+": "+str(x))
print("===")

a=1
while a<n:
    x=pow(g,a)%n
    print(str(a)+" "+str(x))
    a=a+1

    
