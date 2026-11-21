void setup() {
  // put your setup code here, to run once:
  pinMode(2, OUTPUT);
  pinMode(3, OUTPUT);
  pinMode(4, OUTPUT);

  pinMode(6, INPUT_PULLUP);
  pinMode(7, INPUT_PULLUP);
  pinMode(8, INPUT_PULLUP);
  Serial.begin(9600);
}

int array[3];

void loop() {
  // put your main code here, to run repeatedly:
  for (int i = 0; i < 3; i++)
  {
    array[i] = check_connection(i + 2);
  }
  // Serial.print("\n\n\n\n\n\n\n\n\n\n\n\n\n\n\n\n\n\n\n\n\n\n\n\n\n\n\n\n\n\n\n\n\n\n\n\n\n");
  // for (int i = 0; i < 3; i++)
  // {
  //   Serial.print(array[i]);
  // }
  // Serial.print("\n");
  while (Serial.available() > 0)
  {
    if (Serial.read() == 'P')
    {
      puzzlelogic(array);
    }
  }
}


// 2 connects to 6
// 3 connects to 7
// 4 should be disconnected from 8
void puzzlelogic(int *array)
{
  char solves[4] = {'0', '0', '0', '\0'};
  if (array[0] == 6)
  {
    solves[0] = '1';
  }
  if (array[1] == 7)
  {
    solves[1] = '1';
  }
  if (array[2] != 8)
  {
    solves[2] = '1';
  }
  Serial.println(solves);
}

void pins_on()
{
  digitalWrite(2, HIGH);
  digitalWrite(3, HIGH);
  digitalWrite(4, HIGH);
}

int check_connection(int outpin)
{
  pins_on();
  digitalWrite(outpin, LOW);
  delay (1);
  for (int i = 6; i < 9; i++)
  {
    if (digitalRead(i) == LOW)
    {
      return (i);
    }
  }
  return (0);
}