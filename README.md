# GetDependents

此项目用于爬取你感兴趣的项目的 used by 列表，并按 star 降序排序，方便找到有价值的项目学习

# 使用教程

复制你感兴趣的项目的 user by 页面 url，比如
```
https://github.com/radzenhq/radzen-blazor/network/dependents
```
![](https://raw.github.com/qinghuas/GetDependents/master/image/find_url.png)

然后拉取代码，使用 visual studio 打开，修改地址

![](https://raw.github.com/qinghuas/GetDependents/master/image/edit_link.png)

然后运行程序，**注意保持浏览器始终在前台，不然会导致获取到重复的内容**

然后总页面只是一个估算数，因为 github 并没有展示出所有的项目，仅供参考

![](https://raw.github.com/qinghuas/GetDependents/master/image/run_program.png)

运行完成后会在运行目录下生成一个 `projects_<timestamp>.json` 文件，就是爬取的结果了，按 star 数降序排序

![](https://raw.github.com/qinghuas/GetDependents/master/image/view_result.png)

按下 ctrl 键的同时点击链接会自动跳转过去
