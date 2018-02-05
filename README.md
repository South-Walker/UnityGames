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
>* 要注意旋转这个属性不简单是一个三元数（可以用三元数实例化Quaternion）用的是欧拉旋转法，每个参数分别表示绕x，y，z轴旋转度数。<a href="http://blog.csdn.net/candycat1992/article/details/41254799" target="_blank">关于旋转的具体说明，有时间再看</a>

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

#### 2017-12-8

>* <a href="http://blog.csdn.net/lishuzhai/article/details/48501171" target="_blank" title="dachuang">关于粒子效果相关参数说明</a>
>* Duration：动画持续时间（loop关闭）
>* Start Lifetime:粒子存活时间
>* Start Speed:粒子初速度
>* Start Size:初状态粒子大小
>* Emission（（气体）排放，散发）
    >* Rate:每秒发射粒子数量
>* Shape（发射器形状）
>* Color over Lifetime:随粒子存活时间改变的粒子颜色
>* Size over Lifetime:随粒子存活时间改变的粒子大小
>* Renderer（渲染器）

#### 2018-1-27

>* 用旋转矩阵解决应用在不同移动平台下的适应问题
    >* 首先明确一点，对任意向量（x;y;z）做出的有限次平移，缩放，旋转操作后得到的新向量（x';y';z'），可以用旋转矩阵（1）与（x;y;z;1）相乘得到（x';y';z';1），而旋转矩阵（1）可由4*4单位矩阵进行有限次初等行变化得到，只与变化有关，与向量本身无关
    >* 显然，由矩阵乘法的定义，（x;y;z;1）乘以4*4单位矩阵得到（x;y;z;1），即对不变成立
    >* 假设位移后向量为（x+a;y+b;z+c;1），显然，由初等变化与矩阵乘法，只需要对4*4单位矩阵进行3次第三类初等变换即R30(a),R31(b),R32(c)
    >* 假设释放后向量为（ax;by;cz;1），显然，由初等变化与矩阵乘法，只需要对4*4单位矩阵进行3次第2类初等变换即R0(a),R1(b),R2(c)
    >* 旋转的证明较为麻烦，首先应该说明的是，在三维空间中表示刚体的旋转有两种方法：欧拉角与四元数，因为现在用的是旋转矩阵，所以主要用到的是欧拉角
        >* 欧拉角：将物体的旋转拆分成分别以三个轴为转动轴的旋转
            >* 万向节死锁：gimbal万向节在一个平面恰好旋转90度后，存在两平面重合，即两转动轴重合，此刻万向节本身相当于只存在2个转动轴，只有2种转动方式，丧失了1个转动自由度。<a href="http://v.youku.com/v_show/id_XNzkyOTIyMTI=.html" target="_blank" title="dachuang">关万向节锁视频</a>
            >* 在unity里，特别的，定义了旋转父子关系是y-x-z，即先改变y的值，再改变x，最后改变z。当改变到x的值时使得z,y重合，即成为万向节锁，此时，对y与对z的旋转等效。
        >* 旋转矩阵：
            >* Rx(a):[1,0,0,0;0,cos(x),-sin(x),0;0,sin(x),cos(x),0;0,0,0,1]
                >*       1       0       0 0
                >*       0  cos(a) -sin(a) 0
                >*       0  sin(a)  cos(a) 0
                >*       0       0       0 1
            >* Ry(a):[cos(y),0,sin(y),0;0,1,0,0;-sin(y),0,cos(y),0;0,0,0,1]
                >*  cos(a)       0  sin(a) 0
                >*       0       1       0 0
                >* -sin(a)       0  cos(a) 0
                >*       0       0       0 1
            >* Rz(a):[cos(z),-sin(z),0,0;sin(z),cos(z),0,0;0,0,1,0;0,0,0,1]
                >*  cos(a) -sin(a)       0 0
                >*  sin(a)  cos(a)       0 0
                >*       0       0       1 0
                >*       0       0       0 1
            >* 按YXZ的顺序相乘:[cos(y)*cos(z)+sin(x)*sin(y)*sin(z),cos(z)*sin(x)*sin(y)-cos(y)*sin(z), cos(x)*sin(y),0;cos(x)*sin(z),cos(x)*cos(z),-sin(x), 0; cos(y)*sin(x)*sin(z) - cos(z)*sin(y), sin(y)*sin(z) + cos(y)*cos(z)*sin(x), cos(x)*cos(y),0;0,0,0,1]

#### 2018-1-30

>* OnGUI方法在渲染GUI的时候被调用（频繁的），在OnGUI内部使用的方法应该为GUI类的静态方法(也可以设置GUI类的静态属性)例如：
    >* GUI.matrix：设置自适应矩阵
    >* GUI.Button：设置按钮，这个方法有一个bool返回值，在被点击后返回True，捕捉后可以进行处理
    >* GUI.DrawTexture：设置材质（背景）
>* 由于OnGUI方法是频繁调用的，所以切换页面的逻辑就很显然了，即令旧脚本（实际上是MonoBehaviour类的一个子类的实例，可以通过GetComponents<MonoBehaviour>()得到所有脚本的数组）的enabled属性为false，新脚本的enabled属性为true，此时渲染新页面的GUI而非旧页面的，即实现换页（不更换场景）
>* PlayerPrefs这个类下有静态方法可以支持本地持久化保存与读取（SetInt,GetInt,SetFloat,GetFloat,SetString,GetString）
    >* 在不同平台下保存的方法不同，在windows下是以注册表的形式写入的，查看方法是win+r输入regedit，数据存储在HKEY_CURRENT_USER->Software->[CompanyName]->[ProjectName]
    >* CompanyName与ProjectName的查看方法：File=>Build Setting=>Player Settings，在最上方

#### 2018-1-31

>* 切换场景方法：
    >* Application.LoadLevel("SceneName");//5.0版本以前
    >* SceneManager.LoadScene("SceneName");//5.0版本以后，需要引用UnityEngine.SceneManagement命名空间

#### 2018-2-1

>* 移动端拖动页面不换页，松开手指换页的方法
>```c#
>        //判断是否触控
>        if (Input.touchCount > 0)
>        {
>            //获取一个触控点    
>            Touch touch = Input.GetTouch(0);
>            //按钮按下时的回调方法
>            if (touch.phase == TouchPhase.Began)
>            {
>                touchPoint = touch.position;//记录down点
>                prePositon = touch.position;//记录down点                
>            }
>            else if (touch.phase == TouchPhase.Moved)
>            {
>                //touch.position.y - preP          
>                float newPositonY = positionY - touch.position.y + prePositon.y;
>                //positionY的范围-480*7-0
>                positionY = (newPositonY > 0) ? 0 : (newPositonY > (-480 * 7) ? newPositonY : (-480 * 7));
>                //记录上一次的触控点
>                prePositon = touch.position;
>            }
>            else if (touch.phase == TouchPhase.Ended)
>            {
>                //手指抬起来时，图片开始自动移动
>                isMoving = true;
>                //计算从触控开始到抬起的距离
>                currentDistance = (touch.position.y - touchPoint.y) / scale;
>                //执行移动方法      
>                step = (Mathf.Abs(currentDistance) > 150.0f) ? (currentDistance > 0 ? 1 : (-1)) : 0;
>                //计算当前移动步径
>                moveStep = (Mathf.Abs(currentDistance) > 150.0f) ? (currentDistance > 0 ? -Mathf.Abs(moveStep) : Mathf.Abs(moveStep)) : (currentDistance > 0 ? Mathf.Abs(moveStep) : -Mathf.Abs(moveStep));
>                //计算当前编号
>                int newIndex = currentIndex + step;
>                //如果编号超出边界,进行处理
>                //修改当前索引值currentIndex，在OnGUI里渲染
>                currentIndex = newIndex;
>            }
>```

#### 2018-2-1

>* 注意直接在界面上对实例变量赋值的优先度是高于在脚本赋值的

#### 2018-2-4

>* OnGUI最后只在一个挂载脚本中存在，不然可能重叠

#### 2018-2-5

>* 一般的，在unity的Inspector界面中输入的变量只有其特定的几种，为了能够输入自己声明的类型，需要在类型声明前将其对象序列化
>```c#
>	[System.Serializable]
>	public class className
>	{
>        //your class
>   }
>```
>* 在unity中，如果一个物体本身是预置体（Prefab）的副本（名字是蓝色），其具有回复成原先状态的功能，当预置体被删除后，这个功能失效，物体处于Missing状态（名字是棕红色），解决方法是选中物体=>GameObject=>Break Prefab Instance，由于这个物体不再被当做一个Prefab的实例，故功能消失，从Missing状态恢复