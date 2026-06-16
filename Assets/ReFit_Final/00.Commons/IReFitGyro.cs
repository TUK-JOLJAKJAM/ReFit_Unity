using UnityEngine;

//씬마다 자이로센서의 상하좌우입력의 기능을 구현하는 인터페이스
public interface IReFitGyro
{
    //씬마다 상황이 다르니 기본형 처리
    void GyroInputUp() { }
    void GyroInputDown() { }
    void GyroInputLeft() { }
    void GyroInputRight() { }
    void GyroInputCenter() { }
}
