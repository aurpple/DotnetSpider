2015-10-27 Lewis.Zou

1. 删除了Antrl4的部分，以后不需要依赖JAVA JDK了
2. 实现停止触发器, 如按销量倒序采集淘宝数据发现销量为0笔时不再翻页 
3. 添加WebDriver的Login入口
4. Support Chrome 并设置浏览器默认不加载图片（可以做成配置）
5. 修改数据库提交方式为一页一次(之前是一个数据对象一次)
6. ExtractBy中Multi只对Class有效, Property的Multi属性由它自身决定
7. 可以设置WebDriver是否加载图片
8. 可以设定抓取层级
