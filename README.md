# TFSTool
### Winform app
### Purpose
Bridge TFS and Email with custom operation    
枚举TFS中段时间内更新的元素，按表格型式拼接到邮件中，调用本地Outlook程序发送邮件。

### Use Steps:
1. Install Outlook to local machine: avoid local email server prohibit
2. Set Credentials/Email Content in Menu
3. Config filter condition: Date, Type, Status...
4. Retrive TFS data via button 
5. Edit Email content and Reload via Menu 
7. Localize TFS Data via button

### Note 
1. Nlog files in logs folder
2. Localize files in result folder
3. After edit email content, MUST Click Edit Email in Menu which will reload email
4. Send Email via your local email app, like OutLook
