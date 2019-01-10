
import JSEncrypt from 'jsencrypt'

const RSAHelper = {
    encrypt(str){
        let publicKey = 'MIIBIjANBgkqhkiG9w0BAQEFAAOCAQ8AMIIBCgKCAQEAztJWvmn//yWTxEWg5934cftfCumAKUG7D74bsrGjaiTjq6YiL0SE3kYDgpnxJALWui2AXwqej5TItcGlFzS0Kk7MleQT9F3S37rpsI8lPIRL/1iHY2sSLnip9Nu3WDmaZVP54K8uK28NkWImB03J/Qio6o6aUpMyyu9Qt08QPNjB3jcKxGB5XpvfxTcflNEXA7UL86+S4RPL+YbMP2PYGOS0JtWUg/3Rtst3OBq6CZSTt+vRUvDNc37lgcHVVwTZBR44/W+PtfdxiWzIAXGMhhZwfVNB3pwrzsDaL8HEN8KGjDoT6cnqsgRHwB9QnMX2o8uRZgD60Lxl84qbb2qj7QIDAQAB';
        let encryptor = new JSEncrypt();
        encryptor.setPublicKey(publicKey);
        let ciphertext = encryptor.encrypt(str);
        return ciphertext;
    }
}

export { RSAHelper }