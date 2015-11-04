2015-10-30 Lewis.Zou
1. ModelPipelin 改为缓存机制，默认为1即不启用缓存
2. 删除, 合并一些Db相关的类

2015-10-29 Lewis.Zou

1. 可以Sub Html中的Json当成文本源加载到Page
2. 加入CachedPipeline, 实现缓存数据后再操作
3. 修复各种BUG


2015-10-27 Lewis.Zou

1. 删除了Antrl4的部分，以后不需要依赖JAVA JDK了
2. 实现停止触发器, 如按销量倒序采集淘宝数据发现销量为0笔时不再翻页 
3. 添加WebDriver的Login入口, 可以自定义登录网站
4. Support Chrome 
5. 修改数据库提交方式为一页一次(之前是一个数据对象一次)
6. ExtractBy中Multi只对Class有效, Property的Multi属性由它自身决定
7. 可以设置WebDriver是否加载图片
8. 可以设定抓取层级
