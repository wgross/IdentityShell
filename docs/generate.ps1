$types = @(
    "Base64Binary "
    "UpnName"
    "UInteger64"
    "UInteger32"
    "Time"
    "String"
    "Sid"
    "RsaKeyValue"
    "Rsa"
    "Rfc822Name"
    "KeyInfo"
    "Integer64"
    "X500Name"
    "Integer32"
    "HexBinary"
    "Fqbn"
    "Email"
    "DsaKeyValue"
    "Double"
    "DnsName"
    "DaytimeDuration"
    "DateTime"
    "Date"
    "Boolean"
    "Base64Octet"
    "Integer"
    "YearMonthDuration"
)

filter nameof{
    "nameof(ClaimValueTypes.$_),"
}
#$types|nameof|clip


filter sha256base64 {
    $bytes = [System.Text.Encoding]::UTF8.getBytes($_)
    $hash = [System.Security.Cryptography.HashAlgorithm]::Create("SHA256").ComputeHash($bytes)
    [System.Convert]::ToBase64String($hash)
}
var bytes = Encoding.UTF8.GetBytes(input);
var hash = sha.ComputeHash(bytes);

return Convert.ToBase64String(hash);
