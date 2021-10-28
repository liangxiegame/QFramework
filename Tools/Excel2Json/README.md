# Excel2Json

- -e excel文件夹位置
- -j 输出json位置
- -c 输出脚本位置
- -t 脚本模板位置

- example里是案例，bat只有win可以使用，py win和mac都可以用

- 第一行是字段名，第二行是类型，第三行是注释

- 一般的bool ，int，float，string都支持

- array_int为List<int>  其他基本类型类似 输入示例（1|2|3）

- dic_int_string 为Dictionary<int,string> 其他基本类型类似  输入示例（1:a|2:b|3:c）

- //END为结束行，如只想生成前三行，则可在第四行添加//END

- '#' 开头的行会忽略

- sheet如果是默认，则使用文件名当类名，自定义命名则使用自定义的名字做类名，表中可以增加多个sheet
  
- 如果一个sheet的名字是Enum，则会创建对应的枚举
