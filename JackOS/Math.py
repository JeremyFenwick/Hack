def bit_index():
    arr = [1]
    mult = 1
    for i in range(1, 16):
        mult = mult + mult
        arr.append(mult)
    return arr

def index_check( number, index):
    arr = bit_index()
    return (number & arr[index]) != 0

def multiply( x, y):
    total = 0
    shiftedx = x
    for i in range(0, 16):
        if index_check(y, i):
            total = total + shiftedx
        shiftedx = shiftedx + shiftedx
    return total

def divide_core( x, y):
    if y > x:
        return 0
    quotient = divide_core(x, y + y)
    if x - multiply(quotient + quotient, y) < y:
        return 2 * quotient
    else:
        return 2 * quotient + 1

def divide( x, y):
    abs_x = absolute(x)
    abs_y = absolute(y)
    result = divide_core(abs_x, abs_y)
    if (x < 0) != (y < 0):
        return -result
    else:
        return result

def absolute(x):
    if x > 0:
        return x
    else:
        return -x

def power( number, raised_to):
    if raised_to == 0:
        return 1
    result = number
    for i in range(0, raised_to - 1):
        result = (multiply(number, result))
    return result

def square_root( x):
    y = 0
    for i in range(7, -1, -1):
        temp1 = y + (power(2, i))
        if (power(temp1, 2)) <= x:
            y = y + (power(2, i))
    return y
