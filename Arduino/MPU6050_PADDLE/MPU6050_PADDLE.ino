/* I2C device class (I2Cdev) demonstration Arduino sketch for MPU6050 class
// 10/7/2011 by Jeff Rowberg <jeff@rowberg.net>
// Updates should (hopefully) always be available at https://github.com/jrowberg/i2cdevlib
//
// Changelog:
//      2018-04-20 - modified for breakout
//      2013-05-08 - added multiple output formats
//                 - added seamless Fastwire support
//      2011-10-07 - initial release

 ============================================
I2Cdev device library code is placed under the MIT license
Copyright (c) 2011 Jeff Rowberg

Permission is hereby granted, free of charge, to any person obtaining a copy
of this software and associated documentation files (the "Software"), to deal
in the Software without restriction, including without limitation the rights
to use, copy, modify, merge, publish, distribute, sublicense, and/or sell
copies of the Software, and to permit persons to whom the Software is
furnished to do so, subject to the following conditions:

The above copyright notice and this permission notice shall be included in
all copies or substantial portions of the Software.

THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR
IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY,
FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE
AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER
LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM,
OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN
THE SOFTWARE.
===============================================
*/

// I2Cdev and MPU6050 must be installed as libraries, or else the .cpp/.h files
// for both classes must be in the include path of your project
#include "I2Cdev.h"
#include "MPU6050.h"

// Arduino Wire library is required if I2Cdev I2CDEV_ARDUINO_WIRE implementation
// is used in I2Cdev.h
#if I2CDEV_IMPLEMENTATION == I2CDEV_ARDUINO_WIRE
    #include "Wire.h"
#endif

// class default I2C address is 0x68
// specific I2C addresses may be passed as a parameter here
// AD0 low = 0x68 (default for InvenSense evaluation board)
// AD0 high = 0x69
MPU6050 accelgyro;
//MPU6050 accelgyro(0x69); // <-- use for AD0 high

//#define OUTPUT_READABLE_ACCELGYRO
//#define OUTPUT_BINARY_ACCELGYRO

#define MAPPED_XY
//#define RAW_XY

int16_t ax, ay, az;
int16_t gx, gy, gz;

#define LED_PIN 13
bool blinkState = false;

void setup() {
    // join I2C bus (I2Cdev library doesn't do this automatically)
    #if I2CDEV_IMPLEMENTATION == I2CDEV_ARDUINO_WIRE
        Wire.begin();
    #elif I2CDEV_IMPLEMENTATION == I2CDEV_BUILTIN_FASTWIRE
        Fastwire::setup(400, true);
    #endif

    // initialize serial communication
    // (38400 chosen because it works as well at 8MHz as it does at 16MHz, but
    // it's really up to you depending on your project)
    Serial.begin(115200);

    // initialize device
    //Serial.println("Initializing I2C devices...");
    accelgyro.initialize();
    // verify connection
    //Serial.println("Testing device connections...");
    //Serial.println(accelgyro.testConnection() ? "MPU6050 connection successful" : "MPU6050 connection failed");

    accelgyro.setXAccelOffset(-3828);
    accelgyro.setYAccelOffset(890);
    accelgyro.setZAccelOffset(1576);
    accelgyro.setXGyroOffset(117);
    accelgyro.setYGyroOffset(-49);
    accelgyro.setZGyroOffset(52);

    /* 
      From calibration sketch
      Sensor readings with offsets:  2 -1  16379 -1  0 0
      Your offsets: -4007 1014  1633  117 -49 51
      Your offsets:  -3987 1002  1626  118 -49 51
      Your offsets:  -3828 890 1576  117 -49 52

    
    
    Serial.println("Updating internal sensor offsets...");
    Serial.print(accelgyro.getXAccelOffset()); Serial.print("\t"); // 562
    Serial.print(accelgyro.getYAccelOffset()); Serial.print("\t"); // 865
    Serial.print(accelgyro.getZAccelOffset()); Serial.print("\t"); // 1624
    Serial.print(accelgyro.getXGyroOffset()); Serial.print("\t"); // 2200
    Serial.print(accelgyro.getYGyroOffset()); Serial.print("\t"); // 76
    Serial.println(accelgyro.getZGyroOffset());                    // -500

    Serial.print(accelgyro.getXAccelOffset()); Serial.print("\t");
    Serial.print(accelgyro.getYAccelOffset()); Serial.print("\t");
    Serial.print(accelgyro.getZAccelOffset()); Serial.print("\t");
    Serial.print(accelgyro.getXGyroOffset()); Serial.print("\t");
    Serial.print(accelgyro.getYGyroOffset()); Serial.print("\t");
    Serial.print(accelgyro.getZGyroOffset()); Serial.print("\t");
    Serial.print("\n");

*/
    // configure Arduino LED pin for output
    pinMode(LED_PIN, OUTPUT);
}

void loop() {
    delay(100); //slow things down

    // x & y swapped for game, check chip to confirm
    accelgyro.getMotion6(&ax, &ay, &az, &gx, &gy, &gz); //read sensor

    #define RAW_XY

    #ifdef MAPPED_XY 
        #ifdef RAW_XY
          //delay(100);
          Serial.print("\nraw:\t");
          Serial.print(-1 * ay); Serial.print("\t");
          Serial.print(ax); Serial.print("\t");
          Serial.print(-1 * gx); Serial.print("\t");
          Serial.println(gy);
        #endif
        
        Serial.print("a/g:\t");
        Serial.print(-1* map(ay,-32000,32000,-100,100)); Serial.print("\t");
        Serial.print(map(ax,-32000,32000,-100,100)); Serial.print("\t");
        Serial.print(- 1* map(gx,-32000,32000,-100,100)); Serial.print("\t");
        Serial.println(map(gy,-32000,32000,-100,100));
    #endif
    
    // these methods (and a few others) are also available
    //accelgyro.getAcceleration(&ax, &ay, &az);
    //accelgyro.getRotation(&gx, &gy, &gz);

    #ifdef OUTPUT_READABLE_ACCELGYRO
        // display tab-separated accel/gyro x/y/z values
        Serial.print("a/g:\t");
        Serial.print(ax); Serial.print("\t");
        Serial.print(ay); Serial.print("\t");
        Serial.print(az); Serial.print("\t");
        Serial.print(gx); Serial.print("\t");
        Serial.print(gy); Serial.print("\t");
        Serial.println(gz);
    #endif

    #ifdef OUTPUT_BINARY_ACCELGYRO
        Serial.write((uint8_t)(ax >> 8)); Serial.write((uint8_t)(ax & 0xFF));
        Serial.write((uint8_t)(ay >> 8)); Serial.write((uint8_t)(ay & 0xFF));
        Serial.write((uint8_t)(az >> 8)); Serial.write((uint8_t)(az & 0xFF));
        Serial.write((uint8_t)(gx >> 8)); Serial.write((uint8_t)(gx & 0xFF));
        Serial.write((uint8_t)(gy >> 8)); Serial.write((uint8_t)(gy & 0xFF));
        Serial.write((uint8_t)(gz >> 8)); Serial.write((uint8_t)(gz & 0xFF));
    #endif

    // blink LED to indicate activity
    blinkState = !blinkState;
    digitalWrite(LED_PIN, blinkState);
}

