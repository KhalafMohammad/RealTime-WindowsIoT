#include "soc/gpio_struct.h"
#include "driver/gpio.h"
#include "Arduino.h"
#include <HardwareSerial.h>


#define tx_pin 17  // serial comms pins
#define rx_pin 16
#define LED21 21



bool stop1_flag = false;
bool stop2_flag = false;

uint8_t buffer[7];
int16_t motor1_val = 0;

char motor1_dir;
int16_t motor2_val = 0;

char motor2_dir;
char LF;



struct Motor {
  const int PUL_PIN;
  const int DIR_PIN;
};

struct counter {
  const int PIN;
  int counter;
};


Motor Motor1 = { 23, 22 };
Motor Motor2 = { 32, 33 };
counter M1_Counter = { 19, 0 };
counter M2_Counter = { 25, 0 };


void IRAM_ATTR m1_counter() {
  M1_Counter.counter += 1;
}


void IRAM_ATTR m2_counter() {
  M2_Counter.counter += 1;
}


void setup() {
  Serial.begin(115200);
  Serial2.begin(115200, SERIAL_8N1, rx_pin, tx_pin);

  pinMode(M1_Counter.PIN, INPUT_PULLUP);
  pinMode(M2_Counter.PIN, INPUT_PULLUP);

  attachInterrupt(M1_Counter.PIN, m1_counter, RISING);  //motor 1 counter
  attachInterrupt(M2_Counter.PIN, m2_counter, RISING);  //motor 2 counter


  pinMode(Motor1.PUL_PIN, OUTPUT);  //motor 1
  pinMode(Motor1.DIR_PIN, OUTPUT);

  pinMode(Motor2.PUL_PIN, OUTPUT);  //motor 2
  pinMode(Motor2.DIR_PIN, OUTPUT);

  GPIO.enable_w1ts = (1 << 21);
}

void loop() {

  // begin serial comms
  if (Serial2.available() >= 7) {

    // GPIO.out_w1ts = (1 << 21);
    //let led port here blink on the basis of the second byte and then via the osciloscope mesure the time taken from the first bit to the bit(blink)

    Serial2.readBytes(buffer, 7);

    motor1_val = (int16_t)(buffer[0] << 8) | buffer[1];
    motor1_dir = (char)buffer[2];

    motor2_val = (int16_t)(buffer[3] << 8) | buffer[4];
    motor2_dir = (char)buffer[5];

    LF = (char)buffer[6];

    if (motor1_dir == 'E') {  // if E send the positions back
      Serial2.print("m1 ");
      Serial2.print(M1_Counter.counter);
      Serial2.print(" ");
      Serial2.print("m2 ");
      Serial2.print(M2_Counter.counter);
      Serial2.println(" ");
    }

    // directions initalizations
    if (motor1_dir == 'R') {
      digitalWrite(Motor1.DIR_PIN, HIGH);
    } else if (motor1_dir == 'L') {
      digitalWrite(Motor1.DIR_PIN, LOW);
    }

    if (motor2_dir == 'R') {
      digitalWrite(Motor2.DIR_PIN, HIGH);
    } else if (motor2_dir == 'L') {
      digitalWrite(Motor2.DIR_PIN, LOW);
    }

    // speed control
    if (LF == ';') {
      if (motor1_val > 0 && motor1_val <= 2001) {
        analogWriteFrequency(Motor1.PUL_PIN, motor1_val);
        analogWrite(Motor1.PUL_PIN, 16);  //26
        stop1_flag = false;
      } else if (motor1_val == 0) {
        analogWriteFrequency(Motor1.PUL_PIN, 0);
        analogWrite(Motor1.PUL_PIN, 0);  //26
        stop1_flag = true;
      }

      if (motor2_val > 0 && motor2_val <= 2001) {
        analogWriteFrequency(Motor2.PUL_PIN, motor2_val);
        analogWrite(Motor2.PUL_PIN, 16);  //26
        stop2_flag = false;
      } else if (motor2_val == 0) {
        analogWriteFrequency(Motor2.PUL_PIN, 0);
        analogWrite(Motor2.PUL_PIN, 0);  //26
        stop2_flag = true;
      }
    } else {
      stop1_flag = true;
      stop2_flag = true;
      analogWriteFrequency(Motor1.PUL_PIN, 0);
      analogWrite(Motor1.PUL_PIN, 0);  //26
      analogWriteFrequency(Motor2.PUL_PIN, 0);
      analogWrite(Motor2.PUL_PIN, 0);  //26
    }
    // error steps control
    if (LF == '@') {

      for (int i = 0; i < motor1_val; i++) {
        digitalWrite(Motor1.PUL_PIN, HIGH);
        delayMicroseconds(400);
        digitalWrite(Motor1.PUL_PIN, LOW);
        delayMicroseconds(400);
      }

      if (motor1_dir == 'R') {
        M1_Counter.counter += motor1_val;
      } else if (motor1_dir == 'L') {
        M1_Counter.counter -= motor1_val;
      }

      for (int i = 0; i < motor2_val; i++) {
        digitalWrite(Motor2.PUL_PIN, HIGH);
        delayMicroseconds(400);
        digitalWrite(Motor2.PUL_PIN, LOW);
        delayMicroseconds(400);
      }
      if (motor2_dir == 'R') {
        M2_Counter.counter += motor2_val;
      } else if (motor2_dir == 'L') {
        M2_Counter.counter -= motor2_val;
      }

      Serial2.print("e1 ");
      Serial2.print(M1_Counter.counter);
      Serial2.print(" ");
      Serial2.print("e2 ");
      Serial2.print(M2_Counter.counter);
      Serial2.println(" ");
      M1_Counter.counter = 0;
      M2_Counter.counter = 0;
    }
  }
  // GPIO.out_w1tc = (1 << 21);

  if (stop1_flag == true || motor1_dir == 'N') {
    analogWriteFrequency(Motor1.PUL_PIN, 0);
    analogWrite(Motor1.PUL_PIN, 0);  //26

  } else if (stop2_flag == true || motor2_dir == 'N') {
    analogWriteFrequency(Motor2.PUL_PIN, 0);
    analogWrite(Motor2.PUL_PIN, 0);  //26
  }

  stop1_flag = false;
  stop2_flag = false;
  motor1_val = 0;

  motor1_dir = ' ';
  motor2_val = 0;

  motor2_dir = ' ';
  LF = ' ';
}
