# [Unity 3D] HyperCasualAssignments
## 1. 소개

<div align="center">
  <img src="https://github.com/k660323/HyperCasualAssignments/blob/main/Image/%EB%A9%94%EC%9D%B81.JPG" width="33%" height="600"/>
  <img src="https://github.com/k660323/HyperCasualAssignments/blob/main/Image/%EB%B9%B5%20%ED%94%BD%EC%97%85.JPG" width="33%" height="600"/>
  <img src="https://github.com/k660323/HyperCasualAssignments/blob/main/Image/%EB%B9%B5%20%EC%A7%84%EC%97%B4.JPG" width="33%" height="600"/>
  <img src="https://github.com/k660323/HyperCasualAssignments/blob/main/Image/%EA%B3%84%EC%82%B0.JPG" width="33%" height="600"/>
  <img src="https://github.com/k660323/HyperCasualAssignments/blob/main/Image/%EA%B8%B0%EB%AF%B9%20%EC%9A%94%EA%B5%AC.JPG" width="33%" height="600"/>
  <img src="https://github.com/k660323/HyperCasualAssignments/blob/main/Image/%EA%B8%B0%EB%AF%B9%20%EC%98%A4%ED%94%88.JPG" width="33%" height="600"/>
  <img src="https://github.com/k660323/HyperCasualAssignments/blob/main/Image/%EA%B8%B0%EB%AF%B92.JPG" width="33%" height="600"/>
  <img src="https://github.com/k660323/HyperCasualAssignments/blob/main/Image/%EA%B8%B0%EB%AF%B93.JPG" width="33%" height="600"/>
  
  < 게임 플레이 사진 >
</div>

+ HyperCasualAssignments?
  + 과제로 제작한 하이퍼 캐주얼 게임 입니다.
 
+ 목표
  + 플레이어는 빵집 사장이 되어 생산, 운반, 계산 등 모든 일을 도맡아 가게를 운영하고 확장해 나가는 캐주얼 게임 입니다.

+ 게임 흐름
  + 생산된 기초 인프라를 통해 자동으로 빵이 생산된다.
  + 생산된 빵을 플레이어가 직접 들고가 진열대에 배치할 수 있다.
  + 매 단위마다 손님이 입장한다. 입장하는 손님마다 요구사항이 다양하다.
  + 손님은 요구사항이 만족할때 까지 대기하고 만족할시 즉시 다음 행동으로 넘어간다.
  + 매 요구사항을 만족시켜 카운터에 온 손님을 계산하여 돈을 벌수 있으며 점점 가게를 확장하는 것이 목표
     
          

<br>

## 2. 프로젝트 정보

+ 사용 엔진 : UNITY
  
+ 엔진 버전 : 2021.3.15f1

+ 사용 언어 : C#
  
+ 작업 인원 : 1명
  
+ 작업 영역 : 콘텐츠 제작, 디자인, 기획
  
+ 장르      : 하이퍼 캐주얼
  
+ 소개      : 빵집을 운영하는 하이퍼 캐주얼  게임
  
+ 플랫폼    : PC
  
+ 개발기간  : 2024.08.14 ~ 2024.08.20
  
+ 형상관리  : GitHub Desktop

<br>

## 3. 사용 기술
| 기술 | 설명 |
|:---:|:---|
| 디자인 패턴 | ● **싱글톤** 패턴 Managers클래스에 적용하여 여러 객체 관리 <br> ● **FSM** 패턴을 사용하여 플레이어 및 AI 기능 구현|
| Object Pooling | 자주 사용되는 객체는 Pool 관리하여 재사용 |

<br>

## 4. 구현 기능

### **구조 설계**

대부분 유니티 프로젝트에서 사용되고 자주 사용하는 기능들을 구현하여 싱글톤 클래스인 Managers에서 접근할 수 있도록 구현
      
### **코어 매니저**

+ InputManager - 사용자 입력 관리 매니저
+ ParticalManager - 파티클 생성 유틸
+ PoolManager - 오브젝트 풀링 매니저
+ ResourceManager - 리소스 매니저
+ SoundManager - 사운드 매니저

        
### **컨텐츠 매니저**

+ GameManager
  + 게임 컨텐츠에 사용되는 손님 및 기믹 오브젝트를 총 관리
  + 현재 게임 상태 관리
         
[Managers.cs](https://github.com/k660323/HyperCasualAssignments/blob/main/Scripts/Managers/Managers.cs)

<br>

---

<br>
     
### **씬**
게임 씬 단일 씬으로 구성

### **게임 씬**
+ GameScene
  + 게임씬에는 진행할 게임 컨텐츠를 초기화 및 손님을 스폰
    
[GameScene.cs](https://github.com/k660323/FunnyLand/blob/main/Scripts/Scenes/GameScene.cs)

+ EventManager
  + 플레이어가 특정 조건을 만족시 스크립트를 실행하는 매니저
  + 특정 조건이 만족되면 다음 이벤트를 실행한다.
 
[EventManager.cs](https://github.com/k660323/HyperCasualAssignments/blob/main/Scripts/Managers/Contents/EventManager.cs)


+ EventSequence
  + EventManager에서 이벤트를 관리및 활성화 하는데 사용되는 클래스
  + 사용자 지정 클래스를 정의할시 이 클래스를 상속받아 특정 이벤트에 관한 스크립트를 정의한다.
 
[EventSequence.cs](https://github.com/k660323/HyperCasualAssignments/blob/main/Scripts/Contents/EventSequence/EventSequence.cs)


+ Creature
  + 게임 컨텐츠에 사용될 메인 오브젝트 클래스 (플레이어, 손님)
  + 오브젝트 타입, 컨트롤러, 애니메이션, 스탯을 정의

[Creature.cs](https://github.com/k660323/HyperCasualAssignments/blob/main/Scripts/Contents/Creature.cs)


+ Player
  + 플레이어의 현재 상태를 저장하는 Stat 클래스, 플레이어 입력을 관리하는 PlayerController 클래스, 상태를 UI를 표현하는 게임 오브젝트 가지며 이를 초기화 하는 클래스

[Player.cs](https://github.com/k660323/HyperCasualAssignments/blob/main/Scripts/Contents/Player.cs)


+ Customer
  + 손님 로직을 정의한 클래스
  + NavMeshAgent, 고객의 현재 상태를 표현하는 UI 정의 및 초기화 하는 클래스

[Customer.cs](https://github.com/k660323/HyperCasualAssignments/blob/main/Scripts/Contents/Customer.cs)


+ BaseController
  + Creature 오브젝트를 제어하기 위한 추상 클래스
  + FSM 상태 머신에 사용할 변수 선언 및 가상 함수 선언

[BaseController.cs](https://github.com/k660323/HyperCasualAssignments/blob/main/Scripts/Controllers/BaseController.cs)


+ PlayerController
  + 플레이어 상태를 정의한 클래스
  + 각 상태에 대한 기능 구현 (Idle, Move)
 
[PlayerController.cs](https://github.com/k660323/HyperCasualAssignments/blob/main/Scripts/Controllers/PlayerController.cs)


+ CustomerController
  + 손님에 대한 상태를 정의한 클래스
  + 각 상태에 대한 행동 로직 구현

[CustomerController](https://github.com/k660323/HyperCasualAssignments/blob/main/Scripts/Controllers/CustomerController.cs)


+ Gimmick
  + 플레이어 및 손님과 상호작용할 수 있는 추상 클래스
  + 동적 이벤트를 바인딩 가능 ( ex)EventSequence에 관한 로직 )
  + 해당 Type이 어떤 클래스 인지 알기 위해 Enum으로 정의
  + 각 목적에 맞게 상속받아 이를 구현함 (빵 생성기, 빵 진열대, 계산대 등)

[Gimmick](https://github.com/k660323/HyperCasualAssignments/blob/main/Scripts/Contents/Gimmik/Gimmick.cs)


+ Pannel
  + 특정 Gimick을 오픈하기 위한 Pannel
  + 일정 금액을 지불 후 해당 기믹을 스폰하는 형식의 클래스
  + Gimmick와 동일하게 동적 이벤트 바인딩 가능  ( ex)EventSequence에 관한 로직 )
  + 해당 Type이 어떤 클래스 인지 알기 위해 Enum으로 정의
 
[Pannel](https://github.com/k660323/HyperCasualAssignments/blob/main/Scripts/Contents/Panel/Panel.cs)


+ Define
  + 컨텐츠에 사용하는 enum들을 정의한 클래스
 
[Define](https://github.com/k660323/HyperCasualAssignments/blob/main/Scripts/Utils/Define.cs)


+ Util
  + 유용하고 자주 사용하는 함수들을 정의한 클래스
  + 컴포넌트 추가, 게임 오브젝트 찾기, 베지어 커브 등

[Util](https://github.com/k660323/HyperCasualAssignments/blob/main/Scripts/Utils/Util.cs)


+ UI_GameScene
  + 현재 플레이어가 소지한 금액을 UI로 표현해주는 클래스
  + 이벤트 방식으로 데이터 변경시 갱신

[UI_GameScene](https://github.com/k660323/HyperCasualAssignments/blob/main/Scripts/UI/Scene/UI_GameScene.cs)


+ UI_JoyStick
  + 플레이어의 입력을 받는 조이스틱 클래스
  + 터치 이벤트 구현 (터치 되었을시, 터치중, 터치가 끝났을때)
  + 입력된 값은 InputManager의 inputDir값을 갱신 시킨다.

[UI_JoyStick](https://github.com/k660323/HyperCasualAssignments/blob/main/Scripts/UI/SubItem/UI_JoyStick.cs)


<br>

---

<br>

## 5. 구현에 어려웠던 점과 해결과정
+ 기존의 방식으로 새로운 방식과 장르의 게임을 구현하는데 살짝 어려움이 있었습니다.
  + 기존의 정해진 형식과 틀(코딩 방식)에 익숙해져서 이번 게임을 제작하는데 난감했지만 기간안에 구현에 초점을 맞춰 기존의 고정 관념을 깨부수니 빠르게 구현이 가능했습니다.
 
## 6. 느낀점
+ 항상 게임을 구현할때 구조를 잡고 천천히 구현해오다가 이번 과제를 계기로 빠르게 개발할 수 있는 역량을 기를수 있게 되어 좋은 경험이라고 생각합니다.
+ 과제를 따라 만들다 보니 베지어 곡선이라는 새로운 보간 방법을 알게되었습니다.


## 7. 플레이 영상
+ 
