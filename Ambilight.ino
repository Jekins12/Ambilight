#include <Wire.h> 
#include <LiquidCrystal_I2C.h>
#define NUM_LEDS 30
#include "FastLED.h"
#define PIN 2
CRGB leds[NUM_LEDS];

LiquidCrystal_I2C lcd(0x27,16,2);
String inData;
int r, g, b;

void setup() {
  FastLED.addLeds<WS2811, PIN, GRB>(leds, NUM_LEDS).setCorrection( TypicalLEDStrip );
  FastLED.setBrightness(170);
  pinMode(13, OUTPUT);
  Serial.begin(500000);
  lcd.init();
  lcd.backlight();
  lcd.setCursor(0,0);
  lcd.print("Waiting for");
  lcd.setCursor(0,1);
  lcd.print("connection...");
 
}

void loop() {
    
    while (Serial.available() > 0)
    {
        char recieved = Serial.read();
        inData += recieved;
              
              

        if (recieved == 'c'){
          inData="";
          lcd.clear();
          lcd.setCursor(0,0);
          lcd.print("Connected");
          FastLED.setBrightness(170);
          FastLED.show();
          Serial.println("r");
          inData="";


          
        }

        if(recieved == 'd')
            {
              FastLED.setBrightness(0);
              FastLED.show();   
              lcd.clear();
              lcd.setCursor(0,0);
              lcd.print("Disconnected!");
              delay(2000);
              lcd.clear();
              lcd.setCursor(0,0);
              lcd.print("Waiting for");
              lcd.setCursor(0,1);
              lcd.print("connection...");
              inData = "";
            }
        
        

        if (recieved == 'A')
        {
          inData.remove(inData.length() - 1, 1);

          for(int i = 0; i < NUM_LEDS; i++){


              int index = inData.indexOf(" ");
              String R = inData.substring(0,index);
              inData.remove(0,index+1);
              index = inData.indexOf(" ");
              String G = inData.substring(0,index);
              inData.remove(0,index+1);
              index = inData.indexOf(" ");
              String B = inData.substring(0,index);
              inData.remove(0,index+1);
 
              r = R.toInt();
              g = G.toInt();
              b = B.toInt();

                 if(i < NUM_LEDS / 2){
                 leds[i+NUM_LEDS/2]= CRGB(r, g, b);
              }
              
             if (i >= NUM_LEDS / 2){
                 leds[NUM_LEDS-1-i]= CRGB(r, g, b);
              }
             
        }      
        FastLED.show();
        inData= "";
    }    
  }
}
