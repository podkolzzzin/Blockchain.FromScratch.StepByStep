#Encoding
f(x) = y, g(y) = x
#Hash Function
f(data) = code

**Doesn't exist:** g(code) = data 

**Collision:** f(x1) = y, f(x2) = y, x1 != x2

**Probably:** f(x1) = y, f(x2) = y, x1 = x2

**Guarantied:** f(x1) = y, f(x2) = z, y != z, x1 != x2

#Encryption
f(data, key) = cryptotext

**Doesn't exist or too expensive:**
    
* g(cryptotext) = data
* g(cryptotext, data) = key - good encryption

#Asymmetric encryption
* f(data, publicKey) = cryptotext
* g(cryptotext, privateKey) = data

#Digital Sign
* f(data, privateKey) = cryptotext
* g(cryptotext, publicKey) = data => is was encrpyted by private key
##Verify Identity
Alice: Encrypt this, please - randomHash

Bob: f(randomHash, privateKey) = cryptotext

Alice: g(cryptotext, publicKey) = randomHash => It is **definitely** Bob
