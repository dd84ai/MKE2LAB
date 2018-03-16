/*
Пример чтения двоичного файла nvtr.dat
Структура записи: ver1,ver2,ver3,ver4,0,1
Число записей: переменная Ktr файла inf2tr.dat
*/

#define _CRT_SECURE_NO_WARNINGS // отключение предупреждений компилятора о функции fopen

#include <stdio.h>
#include <fstream>
#include <iostream>

using namespace std; // отмена необходимости писать "std::" при работе с потоками

const int Ktr = 4558; // количество конечных элементов (прямоугольников)
const int Count6 = Ktr*6; // количество целых чисел в файле nvtr.dat
const int Count4 = Ktr*4; // количество целых чисел для записи

/* Чтение и запись с использованием функции fopen по 1 числу */
int Read_FromFopen_by1()
{
	int i,r;
	int N6 = 0; // число успешно прочитанных чисел
	int N4 = 0; // число успешно записанных чисел

	printf("\n Функция fopen, по 1 числу: ");

	int ver1,ver2,ver3,ver4;
	int ign0, ign1;

	FILE *binfile_in  = fopen("Z:\\6\\nvtr.dat"     , "rb"); if( !binfile_in  ) return(-1);
	FILE *binfile_out = fopen("Z:\\6\\fopen_by1.bin", "wb"); if( !binfile_out ) { fclose(binfile_in); return(-1); }
	FILE *txtfile_out = fopen("Z:\\6\\fopen_by1.txt", "w" ); if( !txtfile_out ) { fclose(binfile_in); fclose(binfile_out); return(-1); }

	for(i=0; i<Ktr; i++)
		{
			/* читаем 6 чисел записи по 1*/
			r = fread(&ver1, sizeof(ver1), 1, binfile_in);  if(r<1 || ver1<=0) break;
			r = fread(&ver2, sizeof(ver2), 1, binfile_in);  if(r<1 || ver2<=0) break;
			r = fread(&ver3, sizeof(ver3), 1, binfile_in);  if(r<1 || ver3<=0) break;
			r = fread(&ver4, sizeof(ver4), 1, binfile_in);  if(r<1 || ver4<=0) break;
			r = fread(&ign0, sizeof(ign0), 1, binfile_in);  if(r<1 || ign0!=0) break;
			r = fread(&ign1, sizeof(ign1), 1, binfile_in);  if(r<1 || ign1!=1) break;
			N6+=6;

			/* записываем только первые 4 числа записи в текстовый файл*/
			r = fprintf(txtfile_out, "%6d %6d %6d %6d \n", ver1,ver2,ver3,ver4);
			if(r<4) break; // проверка, что запись 4х чисел прошла успешно

			/* записываем только первые 4 числа записи в двоичный файл*/
			r = fwrite(&ver1, sizeof(ver1), 1, binfile_out); if(r<1) break;
			r = fwrite(&ver2, sizeof(ver2), 1, binfile_out); if(r<1) break;
			r = fwrite(&ver3, sizeof(ver3), 1, binfile_out); if(r<1) break;
			r = fwrite(&ver4, sizeof(ver4), 1, binfile_out); if(r<1) break;

			N4+=4;
		}

	fclose(binfile_in);
	fclose(binfile_out);
	fclose(txtfile_out);

	printf(" прочитано=%d, записано=%d ", N6, N4);

	if(N6!=Count6) return(-2);
	if(N4!=Count4) return(-3);
	return(0);
}

// Чтение и запись с использованием функции fopen по 6 чисел
int Read_FromFopen_by6()
{
	int i,r;
	int N6 = 0; // число успешно прочитанных чисел
	int N4 = 0; // число успешно записанных чисел

	printf("\n Функция fopen, по 6 чисел: ");

	struct record6 { int ver1,ver2,ver3,ver4; int ign0,ign1; } rec;

	FILE *binfile_in  = fopen("Z:\\6\\nvtr.dat"     , "rb"); if( !binfile_in  ) return(-1);
	FILE *binfile_out = fopen("Z:\\6\\fopen_by1.bin", "wb"); if( !binfile_out ) { fclose(binfile_in); return(-1); }
	FILE *txtfile_out = fopen("Z:\\6\\fopen_by1.txt", "w" ); if( !txtfile_out ) { fclose(binfile_in); fclose(binfile_out); return(-1); }
 
	for(i=0; i<Ktr; i++)
		{
			/* читаем 6 чисел записи разом*/
			r = fread(&rec, sizeof(rec), 1, binfile_in);
			if(r<1) break;  // проверка, что чтение прошло успешно
			N6 += r * sizeof(rec) / sizeof(int); // т.е. N6+=r*6;

			/* записываем только первые 4 числа записи в текстовый файл*/
			r = fprintf(txtfile_out, "%6d %6d %6d %6d \n", rec.ver1, rec.ver2, rec.ver3, rec.ver4); 
			if(r<4) break; // проверка, что запись 4х чисел прошла успешно
			
			/* записываем только первые 4 числа записи в двоичный файл*/
			r = fwrite(&rec, sizeof(int), 4, binfile_out);
			if(r<4) break; // проверка, что запись прошла успешно

//			r = fwrite(&rec, sizeof(rec), 1, binfile_out);  if(r<1) return(-3); // записываем все 6 чисел записи вместе с игнорируемыми (0 и 1)

			N4+=4;
		}

	fclose(binfile_in);
	fclose(binfile_out);
	fclose(txtfile_out);
	
	printf(" прочитано=%d, записано=%d ", N6, N4);

	if(N6!=Count6) return(-2);
	if(N4!=Count4) return(-3);
	return(0);
}

// Чтение и запись с использованием функции fopen всех чисел разом
int Read_FromFopen_by0()
{
	int i,r;
	int N6 = 0; // число успешно прочитанных чисел
	int N4 = 0; // число успешно записанных чисел

	printf("\n Функция fopen, все  разом: ");

	int *Mem6 = new int[Count6];
	int *Mem4 = new int[Count4];

	FILE *binfile_in  = fopen("Z:\\6\\nvtr.dat"     , "rb"); if( !binfile_in  ) return(-1);
	FILE *binfile_out = fopen("Z:\\6\\fopen_by1.bin", "wb"); if( !binfile_out ) { fclose(binfile_in); return(-1); }
	FILE *txtfile_out = fopen("Z:\\6\\fopen_by1.txt", "w" ); if( !txtfile_out ) { fclose(binfile_in); fclose(binfile_out); return(-1); }
 
	/* читаем все чисела разом */
	N6 = fread(Mem6, sizeof(Mem6[0]), Count6, binfile_in);
	if(N6!=Count6) goto end;
	for(i=0, r=0; r<Count6; r+=6, i+=4)
	{
		Mem4[i+0] = Mem6[r+0]; // ver1
		Mem4[i+1] = Mem6[r+1]; // ver2
		Mem4[i+2] = Mem6[r+2]; // ver3
		Mem4[i+3] = Mem6[r+3]; // ver4
	}

	/* записываем все чисела разом */
	r = fwrite(Mem4, sizeof(Mem4[0]), Count4, binfile_out);
	if(r!=Count4) goto end;

	for(i=0; i<Count4; i+=4)
		{
			r = fprintf(txtfile_out, "%6d %6d %6d %6d \n", Mem4[i], Mem4[i+1], Mem4[i+2], Mem4[i+3]);
			if(r<4) break; // проверка, что запись 4х чисел прошла успешно

			N4+=4;
	}

end:	
	delete Mem6;
	delete Mem4;

	fclose(binfile_in);
	fclose(binfile_out);
	fclose(txtfile_out);

	printf(" прочитано=%d, записано=%d ", N6, N4);

	if(N6!=Count6) return(-2);
	if(N4!=Count4) return(-3);
	return(0);
}

// Чтение и запись с использованием класса stream по 1 числу
int Read_FromStream_by1()
{
	int i;
	int N6 = 0; // число успешно прочитанных чисел
	int N4 = 0; // число успешно записанных чисел

	cout << endl << " Класс stream, по 1 числу:  ";

	int ver1,ver2,ver3,ver4;
	int ign0, ign1;

	ifstream binfile_in ("Z:\\6\\nvtr.dat"      , ios::binary); if( !binfile_in.is_open()  ) return(-1);
	ofstream binfile_out("Z:\\6\\stream_by1.bin", ios::binary); if( !binfile_out.is_open() ) { binfile_in.close(); return(-1); }
	ofstream txtfile_out("Z:\\6\\stream_by1.txt"             );	if( !txtfile_out.is_open() ) { binfile_in.close(); binfile_out.close(); return(-1); }

	for(i=0; i<Ktr; i++)
		{
			/* читаем 6 чисел записи по 1*/
			binfile_in.read( (char*)&ver1, sizeof(ver1) ); if(binfile_in.gcount() != sizeof(int)) break;
			binfile_in.read( (char*)&ver2, sizeof(ver2) ); if(binfile_in.gcount() != sizeof(int)) break;
			binfile_in.read( (char*)&ver3, sizeof(ver3) ); if(binfile_in.gcount() != sizeof(int)) break;
			binfile_in.read( (char*)&ver4, sizeof(ver4) ); if(binfile_in.gcount() != sizeof(int)) break;
			binfile_in.read( (char*)&ign0, sizeof(ign0) ); if(binfile_in.gcount() != sizeof(int)) break;
			binfile_in.read( (char*)&ign1, sizeof(ign1) ); if(binfile_in.gcount() != sizeof(int)) break;
			N6+=6;

			/* записываем только первые 4 числа записи в текстовый файл*/
			txtfile_out << ver1 << " " << ver2 << " " << ver3 << " " << ver4 << endl;
			if( txtfile_out.fail() ) break; // проверка, что запись 4х чисел прошла успешно

			/* записываем только первые 4 числа записи в двоичный файл*/
			binfile_out.write( (char*)&ver1, sizeof(ver1) ); if( binfile_in.gcount() != sizeof(int) ) break;
			binfile_out.write( (char*)&ver2, sizeof(ver2) ); if( binfile_in.gcount() != sizeof(int) ) break;
			binfile_out.write( (char*)&ver3, sizeof(ver3) ); if( binfile_in.gcount() != sizeof(int) ) break;
			binfile_out.write( (char*)&ver4, sizeof(ver4) ); if( binfile_in.gcount() != sizeof(int) ) break;

			N4+=4;
		}

	binfile_in.close();
	binfile_out.close();
	txtfile_out.close();

	cout << " прочитано=" << N6 << ", записано=" << N4;

	if(N6!=Count6) return(-2);
	if(N4!=Count4) return(-3);
	return(0);
}

// Чтение и запись с использованием класса stream по 6 чисел
int Read_FromStream_by6()
{
	int i;
	int N6 = 0; // число успешно прочитанных чисел
	int N4 = 0; // число успешно записанных чисел

	cout << endl << " Класс stream, по 6 чисел:  ";

	struct record6 { int ver1,ver2,ver3,ver4; int ign1,ign2; } rec;

	ifstream binfile_in ("Z:\\6\\nvtr.dat"      , ios::binary); if( !binfile_in.is_open()  ) return(-1);
	ofstream binfile_out("Z:\\6\\stream_by1.bin", ios::binary); if( !binfile_out.is_open() ) { binfile_in.close(); return(-1); }
	ofstream txtfile_out("Z:\\6\\stream_by1.txt"             );	if( !txtfile_out.is_open() ) { binfile_in.close(); binfile_out.close(); return(-1); }

	for(i=0; i<Ktr; i++)
		{
			binfile_in.read( (char*)&rec, sizeof(rec) );
			N6 += binfile_in.gcount() / sizeof(int);

			/* записываем только первые 4 числа записи в текстовый файл*/
			txtfile_out << rec.ver1 << " " << rec.ver2 << " " << rec.ver3 << " " << rec.ver4 << endl;
			if( txtfile_out.fail() ) break; // проверка, что запись 4х чисел прошла успешно

			/* записываем только первые 4 числа записи в двоичный файл*/
			binfile_out.write( (char*)&rec, 4*sizeof(int) ); 
			if( binfile_in.fail() ) break;

			N4+=4;
		}

	binfile_in.close();
	binfile_out.close();
	txtfile_out.close();

	cout << " прочитано=" << N6 << ", записано=" << N4;

	if(N6!=Count6) return(-2);
	if(N4!=Count4) return(-3);
	return(0);
}

// Чтение и запись с использованием класса stream  всех чисел разом
int Read_FromStream_by0()
{
	int i;
	int N6 = 0; // число успешно прочитанных чисел
	int N4 = 0; // число успешно записанных чисел

	cout << endl << " Класс stream, все  разом:  ";

	int *Mem6 = new int[Count6];
	int *Mem4 = new int[Count4];

	ifstream binfile_in ("Z:\\6\\nvtr.dat"      , ios::binary); if( !binfile_in.is_open()  ) return(-1);
	ofstream binfile_out("Z:\\6\\stream_by1.bin", ios::binary); if( !binfile_out.is_open() ) { binfile_in.close(); return(-1); }
	ofstream txtfile_out("Z:\\6\\stream_by1.txt"             );	if( !txtfile_out.is_open() ) { binfile_in.close(); binfile_out.close(); return(-1); }

	/* читаем все чисела разом */
	binfile_in.read( (char*)Mem6, Count6*sizeof(int) );  
	N6 = binfile_in.gcount() / sizeof(int);
	if( binfile_in.fail() || N6!=Count6 ) goto end;
	for(i=0; i<Ktr; i++)
	{
		Mem4[i*4+0] = Mem6[i*6+0]; // ver1
		Mem4[i*4+1] = Mem6[i*6+1]; // ver2
		Mem4[i*4+2] = Mem6[i*6+2]; // ver3
		Mem4[i*4+3] = Mem6[i*6+3]; // ver4
	}

	/* записываем все чисела разом */
	binfile_out.write( (char*)Mem4, Count4*sizeof(int) );
	if( binfile_out.fail() ) goto end; // проверка, что запись всех чисел прошла успешно
	
	for(i=0; i<Count4; i+=4)
		{
			/* записываем только первые 4 числа записи в текстовый файл*/
			txtfile_out << Mem4[i]<< " " << Mem4[i+1] << " " << Mem4[i+2] << " " << Mem4[i+3] << endl;
			if( txtfile_out.fail() ) break; // проверка, что запись 4х чисел прошла успешно

			N4+=4;
		}

end:	
	delete Mem6;
	delete Mem4;

	binfile_in.close();
	binfile_out.close();
	txtfile_out.close();

	cout << " прочитано=" << N6 << ", записано=" << N4;

	if(N6!=Count6) return(-2);
	if(N4!=Count4) return(-3);
	return(0);
}

void main()
{
//	setlocale(LC_ALL,"Russian");
	setlocale(0,""); // установка русского языка для вывода
	
	printf("Число КЭ = %d \n", Ktr);
	printf("Число хранящихся чисел = %d \n", Count6);
	printf("Число чисел для записи= %d \n", Count4);

	if( Read_FromFopen_by1() < 0 ) printf(" ОШИБКА! ");
	if( Read_FromFopen_by6() < 0 ) printf(" ОШИБКА! ");
	if( Read_FromFopen_by0() < 0 ) printf(" ОШИБКА! ");
	if( Read_FromStream_by1() < 0 ) printf(" ОШИБКА! ");
	if( Read_FromStream_by6() < 0 ) printf(" ОШИБКА! ");
	if( Read_FromStream_by0() < 0 ) printf(" ОШИБКА! ");

	printf("\n\nPress any key: ");
	getchar();
}
