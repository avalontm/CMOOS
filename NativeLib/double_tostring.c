#include "..\LibC\printf.h"

void double_tostring(double value, char* buffer)
{
	snprintf(buffer, 22, "%lf", value);
}