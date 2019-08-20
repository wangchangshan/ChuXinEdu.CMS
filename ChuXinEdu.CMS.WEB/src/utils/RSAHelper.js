
import JSEncrypt from 'jsencrypt'

const RSAHelper = {
    encrypt(str){
        let publicKey = 'MIIBIjANBgkqhkiG9w0BAQEFAAOCAQ8AMIIBCgKCAQEA+MNImTs5P0uQ3r4Cr0rTL5iOwFfeMTjlnD7nj9Ebpx1h80xwnYm0L9/69A6vITELHLL+ESgwYBxcGb4IKT7wFXID/qcQhmwdjib5JwkStJgL+MaNSorTE+AESEg+A+KKR2vDZs5jo3ZcUbtaezs1OuwX9/pKm6/YvKWqi37bOAWVAPena+uvdruYkTRXgm+k8Vd8jeX0cxjv8O/MxebxutW40V8FzLogPx9t7oWZsBN3ODz7Q+MVkhoEwtIJB/lxmnb2sNO36qgWm4ae0i29gIzYGui7INcQ2mU8wVgMoBXOMCWcUxe2LeRbxqSkE+uqfL+kdZVTUZIA9nXLNVv6cQIDAQAB';
        let encryptor = new JSEncrypt();
        encryptor.setPublicKey(publicKey);
        let ciphertext = encryptor.encrypt(str);
        return ciphertext;
    }
}

export { RSAHelper }