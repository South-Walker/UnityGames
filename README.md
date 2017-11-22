UnityNote
===

unity学习笔记
---

> 彼时我见如来，此时如来见我

#### 2017-11-13

>```c#
>this.transform.position = Vector3.Lerp(aim, now, Time.deltaTime * Smooth);
>```
>* 线性插值方法，实际上是从now点出发在获得一个接近aim的比，now与aim距离之值 比上 返回值与aim距离之值等于第三个参数，用这个方法可以实现摄像机弹性的跟踪一个移动目标

>```c#
>this.transform.rotation = Quaternion.Euler(5, 0, 0);
>```
>* 要注意旋转这个属性不简单是一个三元数（可以用三元数实例化Quaternion）用的是欧拉旋转法，每个参数分别表示绕x，y，z轴旋转度数。<a href="http://blog.csdn.net/candycat1992/article/details/41254799" target="_blank" title="dachuang">关于旋转的具体说明，有时间再看</a>

#### 2017-11-14

>```c#
>IAnimatorable.SetFloat(string name, float value, float dampTime, float deltaTime);
>```
>* 实现了IAnimatorable接口的类都有SetFloat这个实例方法，表示对该类中名字为name的属性赋值为value，后面两个参数可选，其中dampTime为到达value允许的最低时间（帧刷新需要时间），deltatime输入帧刷新时间，换句话说这一帧结束时的速度value满足
>```c#
>value = current + (target - current) * deltaTime / (dampTime + deltaTime); 
>```

>```c#
>Vector3 front = this.transform.forward;
>```
>* transform的forward这个属性返回的是当前对象z轴在大坐标上对应向量的单位向量，注意在Unity的注释里，是用颜色表示笛卡尔坐标轴的，blue axis指的就是z轴

#### 2017-11-15

>* 初步解决了基本的运动（前、左、右），但是当物体朝向（this.transform.forward）改变时原有算法失效。解决思路如下：
>* 以物体本身建系，表示物体朝向的矢量在方向上等于，过起点和终点（该帧起始和结束），圆心角为锐角的一段弧对应的圆在终点处的切线。
>* 同时，当帧间隔很短时，弧长也很短，可以近视做一条线段，对应的切线与线段重合，
>* 举个例子，当帧间隔很短时，物体朝向对应的矢量在方向上等于物体在这一帧内位移对应的矢量
>```c#
>this.transform.forward = nextposition - this.transform.position;
>```
>* 最后，要修改位移算法，因为此时物体朝向会改变，所以计算物体位移时以物体朝向为z轴建系后得到的位移矢量要换算成主坐标系的位移矢量，具体计算方法如下：
>* 首先声明的一点this.transform.forward得到的是单位矢量，长度恒为1，另其为α
>* 令以物体朝向为z轴得到的坐标系为小坐标系，先以小坐标系为准，用角度和速度得到参考位移矢量β，β在小坐标系z轴上投影为b，在数值上等于|α·β|，所以物体在z轴方向位移矢量等于|α·β|α
>* 假设矢量γ⊥α，所以有|γ·α|=0，γ可解，物体在伽马方向位移矢量等于|β·γ|γ
>* 故矢量β在大坐标系上等于|α·β|α+|β·γ|γ
>* (其实上面是算错了，多算了一个叉积导致只有当大小坐标系z轴方向相同时结果才正确，正确的算法见11-18笔记)
>* 但是，以上算法有个缺点，由于朝向已经和Direction属性绑定，而Direction是基于输入的，所以在无输入时，朝向恢复默认值。
>* 解决方法:
>```c#
>Rotation += newdirection;//之前相当于是用向量的方向来表示，现在是与direction值本身持续的时间和大小挂钩
>```

#### 2017-11-17

>```c#
>Vector3.Cross(nowforward, blueaxis) == Vector3.Cross(beforeforward, blueaxis);
>Vector3.Dot(nowforward, blueaxis) <= Vector3.Dot(beforeforward, blueaxis);
>```
>* 前者利用外积判断两单位向量是否在z轴同侧，后者利用内积判断nowforward与z轴成的角是否大于beforeforward与z轴成的角

#### 2017-11-18

>* 使前进方向与朝向挂钩的算法：
>```c#
>float nextx = _speed * Mathf.Cos(rad);
>float nextz = _speed * Mathf.Sin(rad);
>Vector3 front = this.transform.forward;
>Vector3 right = Vector3.Cross(front, new Vector3(0, 1, 0));
>Vector3 nextstep = nextz * front + nextx * right;
>```

#### 2017-11-19

>```c#
>public GameObject GameObject;
>GameObject.GetComponent<ScriptName>().AttributionOrMethor
>```
>* 用这种泛型形式在别的脚本中调用ScriptName脚本中的属性或方法

#### 2017-11-20

>* 生成exe的方法File=>Build Settings

>* 判断当前动画播放状态（AnimatorStateInfo类）
>```c#
>private bool haveJumped()
>{
>   var animatorStateInfo = animator.GetCurrentAnimatorStateInfo(0);
>   if (animatorStateInfo.IsName("Jump") && animatorStateInfo.normalizedTime > 0.8)
>   {
>       willJump = false;
>       return true;
>   }
>   return false;
>}

#### 2017-11-22

>* 当需要做操控角色的碰撞检测时，用Character Controller比添加为刚体更为方便，此时，使this运动不再直接对this.transform.position赋值，而使用如下方法：
>```c#
>public CharacterController CharacterController;
>CharacterController = this.GetComponent<CharacterController>();
>CharacterController.move(GetNextStep());
>```
>* Character Controller与刚体重力不兼容，同时使用会造成物体震颤，解决方法是使用isGrounded判断是否在地面，如果不是，维护一个表示向下速度的属性，在帧末尾执行下落操作
>* Character Controller里Skin Width是个比较重要的属性，表示允许别的碰撞器进入多少距离，有待研究
