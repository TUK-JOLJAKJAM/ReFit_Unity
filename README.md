# ReFit
- 게임화 스마트 재활 시스템
- Gamification Smart Rehabilitation System
- 목적 : 지루한 반복 재활 동작을 게임의 조작으로 바꾸어 재활 동작을 재미있게, 더 효과적으로 할 수 있게 유도

# 게임 구성
### 탐험
- 탐험하면서 마주치는 적들을 물리치며 앞으로 나아가는 게임
- 적의 약점 속성의 운동 스킬을 사용하여 적에게 강한 데미지를 입히는게 중요한 게임
- 부위 : 어깨, 허리, 팔, 손목 등 다양한 부위
- 재활 목표 : 하루에 정해진 양만큼 다양한 운동 진행

## 재활 흐름
1. 탐험을 통해 매일 꾸준히 재활 운동 진행
2. 장작패기, 성벽 지키기 로 일상생활에 주로 필요한 어깨와 허리를 필요한 만큼 추가 운동
3. 쌓인 데이터를 통해 필요하거나 부족한 운동 파악
4. 파악된 데이터로 게임 밸런스 조절
5. 1~4 반복으로 사용자 맞춤 재활 게임 제공

---

<개발자용 설명>

# 디바이스 세팅
## 데모파일
1. pc와 디바이스용 폰을 같은 네트워크에 접속시킴(학교 네트워크 X -> 보안 빡셈)
2. [PC] powerShell에 ipconfig입력해서 무선랜 ip주소 확인
3. [Mobile] phoneGyro 어플 실행해서 입력란에 확인한 무선랜 ip주소 입력 -> 왼쪽 하단에 target changed.... 텍스트 확인
4. [Mobile] 휴대폰을 양팔로 가로로 든 뒤 팔을 쭉 뻗은 상태에서 가운데 Reset 버튼 누름
5. [PC] 데모 파일 실행 후 네트워크 알림 허용 누르기 (안하면 방화벽 끄거나 설정 들어가서 허용으로 바꿔야 됨)

## 유니티
1. 휴대폰으로 조작할 경우 4번까지 동일. 진행 후 유니티 상단의 플레이 누르기
<img width="728" height="410" alt="image" src="https://github.com/user-attachments/assets/04dd5b5f-c47d-4ce8-9e8b-d448235f36f2" />

2. 휴대폰으로 조작할 수 없는 경우 테스트 모드 켜기(GameManager -> TestHandler -> isTestMode 체크)
<img width="728" height="410" alt="image" src="https://github.com/user-attachments/assets/29d577e1-7234-4861-8527-5d24f51e9110" />

2.1 테스트 모드를 켜면 게임 실행 시 가운데 빨간 원이 생긴다. 빨간 원 내부에서 X축, Y축의 값이 -1 ~ +1 로 변한다. 그 바깥은 다 동일하게 최댓값이다.
<img width="734" height="412" alt="image" src="https://github.com/user-attachments/assets/38c58ba9-1daa-4a61-8e1d-5aed4c56be22" />

# 게임 흐름
- 일정시간 자이로값을 입력해야 해당 방향의 입력이 생김
## Title
<img width="728" height="410" alt="image" src="https://github.com/user-attachments/assets/6f0578ec-4adc-401f-bd62-73a2e90ef340" />

- 입력 시간 2초
- 위 입력 : 메인 씬으로 넘어감

## MainScene
<img width="717" height="405" alt="image" src="https://github.com/user-attachments/assets/0caf9b8d-07a1-43ef-be90-5fd487c72959" />

- 입력 시간 1초
- 왼쪽 입력 : 집 하나씩 왼쪽으로 이동 (Fight까지 가면 이동하지 않음)
- 오른쪽 입력 : 집 하나씩 오른쪽으로 이동 (Options까지 가면 이동하지 않음)
- 위 입력 : 해당 메뉴로 이동
  - Fight : 모험 게임 시작
  - Profile : 프로필 UI 띄우기 (구현 안 됨)
  - Options : 옵션 UI 띄우기 (구현 안 됨)

## AdventureScene
<img width="735" height="403" alt="image" src="https://github.com/user-attachments/assets/03ce70b2-b248-48c4-b09e-9aa7251f60e0" />

- 입력 시간 1초
- 위 입력 : 현재 스테이지 진입
- 아래 입력 : 메인 씬으로 이동

## FightScene -> SkillSelect
<img width="733" height="408" alt="image" src="https://github.com/user-attachments/assets/d9069b4c-89d1-4d27-bd71-44282357b384" />

- 입력 시간 1초
- 왼쪽 입력 : 왼쪽 스킬 선택
- 오른쪽 입력 : 오른 쪽 스킬 선택
- 위 입력 : 해당 스킬 사용(Attack단계로 넘어감)

## FightScene -> Attack
<img width="732" height="407" alt="image" src="https://github.com/user-attachments/assets/e1650d7a-7eee-40c1-957d-2d487e1dacaf" />

- 입력 시간 없음
- 스킬 별 동작에 맞춰서 스킬 5회 사용.
- 차징 후 특정 시간 안에 자세를 풀어야 사용된다. (이 반응 속도도 데이터로 넘김)
- 빨간색(상완이두근) : 이두컬 자세로 끝까지 올리면 차징, 차징 후 다시 일자로 펴면 공격
- 파란색(허리) : 팔을 앞으로 쭉 핀 상태에서 한 쪽으로 허리를 돌리면 차징, 차징 후 다시 가운데로 오면 공격(오른쪽, 왼쪽 번갈아서)
- 초록색(어깨) : 팔을 앞으로 쭉 핀 상태에서 위쪽으로 올리면 차징(어깨피기), 차징 후 다시 가운데로 오면 공격
- TestMode : y축 높게 입력 시 차징, 아래로 입력 시 공격
- 적이 죽지 않을 경우 Guard 단계로 넘어감

## FightScene -> Guard
<img width="726" height="402" alt="image" src="https://github.com/user-attachments/assets/1bef0b76-a821-4248-bb43-9683bb55a422" />

- 입력 시간 없음
- 떨어지는 공격들을 방패로 막으면 실드 카운드 증가, 실드 카운트 만큼 공격을 경감해서 맞음
- 팔을 앞으로 쭉 편 채로 운전대 움직이듯이 좌우로 기울이면 방패 이동
- TestMode : x축 입력 하면 이동
- 플레이어가 죽지 않을 경우 SkillSelect 단계로 넘어감 (전투 루프 진행)

## Win
(사실상 Result UI. Lose를 따로 만들어놨지만 그냥 결과창 이거 하나 써도 무방할 것으로 보임 결과에 따라 입력 선택지만 다르게 주고)
<img width="735" height="409" alt="image" src="https://github.com/user-attachments/assets/0b031a0b-71f0-4a75-b809-67db45741ea3" />

- 입력 시간 1초
- 위 입력 : AdventureScene으로 이동
(Lose시에는 MainScene으로 이동하게 하면 될 것 같음)

# 수정 및 추가 필요 내용
[다른 파트와 무관]
1. [-][공통] 자이로 입력 차징 GUI 필요. 얼마나 더 해당 방향으로의 입력을 주어야 입력이 되는 지가 표시되어야 UX가 좋음.
2. [o][공통] 현재 어떤 입력이 가능한지 알려주는 GUI 필요.
3. [o][FightScene] SkillSelect 스킬 선택 순서 좀 꼬여서 수정 필요.
4. [o][FightScene] Attack에서 공격 시 판정을 알려주는 GUI필요.
5. [o][FightScene] 적 HP GUI 필요.
6. [o][FightScene] 적 약점을 표시하는 GUI 필요.
7. [-][FightScene] 현재 스킬의 동작이 어떤 동작인지 알려주는 GUI 필요.
8. [o][FightScene] Win후 AdventureScene으로 넘어갈 때 챕터 이동 필요.(stageLevel 변수 값과 화살표 UI 위치만 옮기면 됨)
9. [-][FightScene] Boss 스테이지 모델 바꾸기
10. [o][MainScene] Profile, Options UI 만들기
11. [o][MainScene] 게임 종료 만들기
12. [-]게임 구성 좀 더 다채롭게 하기. 스테이지 이기고 나서 스텟을 올릴 수 있게 한다던가, 스킬을 업그레이드 한다던가
13. [-]모바일 앱 이쁘게 만들기;;;
14. [o]사운드 넣기
15. [-]방향으로 입력하는 구조다 보니까 위 입력이 넘어간 곳에서 바로 입력되는 경우가 있다. 이걸 방지하는 입력 구조를 만들어야할 것 같음. 가운데에 확인 버튼을 입력해야 한다던지.

[다른 파트와 관련 있는 부분]
1. [-]웹에서 로그인하면 토큰 전달받아서 유니티 실행 시 자동으로 토큰 가지고 있게 하기
2. [-]AdventureManager에서 스테이지 노드 만들 때 모두 동일한 확률로 랜덤으로 만들고 있지만, 사용자 분석 해서 좀 더 운동이 필요한 부위의 등장 확률를 늘리는 등 가중치 부여 필요.
3. [-]사용자 정보 가져와서 각 부위 ROM등 개인별 신체 정보를 이용한 각도 보정치 필요함. (일반적으로 어깨 -90 ~ 90 이 -1 ~ 1 이면 아픈 사람은 -90 ~ 70이 -1 ~ 1 이라던지)
4. [-]분석에 쓰일 수 있게끔 모바일어플에서 자이로 값 뿐만 아니라 가속도 센서 도 넘기게 하기.(필요하면)

# 수정 시 주의 내용
1. Inspector에 할당해서 코드에서 사용하는 public 변수 (컴포넌트나 GameObject)는 자료형을 변경하거나 하면 Inspector에서 한 할당이 풀릴 수 있다. 그런 경우 다시 할당해줘야 한다.
2. 코드에서 ChildrenCount를 사용하는 경우, 혹은 transform.GetChild[0]같이 인덱스로 자식을 불러오는 경우, 그 부모 오브젝트의 자식의 순서를 바꾸거나 자식을 추가, 삭제 하면 코드가 꼬일 수 있다. UIManager의 경우 방지코드를 써놨는데, 다른 곳에선 안해놨을 수도 있으니 조심

# 백엔드 수정 요청
- [o]GameHistory저장할 때 primaryPart입력 부분 ("SHOULDER", "WAIST"등...)에 "BICEPS_BRACHII" 추가
