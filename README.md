# DomainPasswordTest
Tests AD passwords while respecting Bad Password Count

# Options
* --Password(-p) - Password to test
* --LdapPort - Port LDAP is running on (Default: 389)
* --DomainController - Domain controller to connect too (Default: PDC)
* --Lockout - Password lockout threshold (Default: 5)
* --Verbose - Verbose Output

# Usage
Get password lockout threshold using **net accounts /domain** or other alternatives. The program will attempt to keep badpwdcount to lockout value - 2

> DomainPasswordTest.exe -p Password123! --Lockout <value>
