# ReFit Unity 데이터 계약 v2

전투 판정, 게이지 임계값과 몬스터 로직은 그대로 유지하고 공격 동작 중 센서 시퀀스를 30Hz로 부가 수집합니다.

## 전송 데이터

- 세션: `schemaVersion=2.0`, 실제 epoch milliseconds, 주 운동 부위, 난이도, 테스트 모드
- 동작: ID, 운동 코드, 시작·종료·소요시간, 성공 여부, 공격 등급, 반응시간
- 센서 샘플: epoch milliseconds와 보정된 quaternion `qx/qy/qz/qw`
- 미측정 통증·피로·ROM은 임의의 0이나 고정값으로 만들지 않고 `null`/빈 metrics로 전송

운동 코드 매핑:

| Unity 스킬 | 운동 코드 | 분석 부위 |
|---|---|---|
| Red | `BICEPS_CURL` | `BICEPS_BRACHII` |
| Green | `SHOULDER_FLEXION` | `SHOULDER` |
| Blue | `WAIST_ROTATION` | `WAIST` |

## 서버와 계정 설정

빌드를 다시 만들지 않고 다음 우선순위로 설정을 읽습니다.

1. 실행 인자
2. 환경 변수
3. `Application.persistentDataPath/refit-runtime.json`
4. Inspector의 기존 HTTP 데모 기본값

설정 파일 예시:

```json
{
  "apiBaseUrl": "http://43.200.20.216",
  "email": "unity-test@example.com",
  "password": "test-password"
}
```

실행 인자 예시:

```text
--refit-api-base=http://43.200.20.216 --refit-email=unity-test@example.com --refit-password=test-password
```

환경 변수는 `REFIT_API_BASE_URL`, `REFIT_EMAIL`, `REFIT_PASSWORD`, `REFIT_ACCESS_TOKEN`, `REFIT_REFRESH_TOKEN`을 지원합니다. 설정이 없으면 기존 데모 계정으로 로그인해 현재 시연 빌드와 호환됩니다.

## 회귀 확인

- Unity Editor에서 스크립트 컴파일 오류 0건 확인
- 테스트 모드에서 기존 공격 성공/실패 횟수와 점수가 변경되지 않는지 확인
- 플레이 종료 후 Spring 기록 상세의 `schemaVersion`이 `2.0`인지 확인
- `gameData[*].samples`가 3개 이상이며 timestamp가 증가하는지 확인
- React에서 기록 선택 후 `analysis_version=rules-2.0.0`, `data_quality.assessable=true`인지 확인
