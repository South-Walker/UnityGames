UnityNote
===

unity学习笔记
---

> 山就在那里

#### 2017-11-13

>```c#
>this.transform.position = Vector3.Lerp(aim, now, Time.deltaTime * Smooth);
>```
>* 线性插值方法，实际上是从now点出发在获得一个接近aim的比，now与aim距离之值 比上 返回值与aim距离之值等于第三个参数，用这个方法可以实现摄像机弹性的跟踪一个移动目标

>```c#
>this.transform.rotation = Quaternion.Euler(5, 0, 0);
>```
>* 要注意旋转这个属性不简单是一个三元数（可以用三元数实例化Quaternion）用的是欧拉旋转法，每个参数分别表示绕x，y，z轴旋转度数。<a href="http://blog.csdn.net/candycat1992/article/details/41254799" target="_blank" title="dachuang">关于旋转的具体说明，有时间再看</a>

#### 2017-11-13

>```c#
>IAnimatorable.SetFloat(string name, float value, float dampTime, float deltaTime);
>```
>* 实现了IAnimatorable接口的类都有SetFloat这个实例方法，表示对该类中名字为name的属性赋值为value，后面两个参数可选，其中dampTime为到达value允许的最低时间（帧刷新需要时间），deltatime输入帧刷新时间，换句话说这一帧结束时的速度value满足
>```c#
>value = current + (target - current) * deltaTime / (dampTime + deltaTime); 
>```
