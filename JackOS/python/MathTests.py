import unittest

from Math import *


class Tests(unittest.TestCase):
    def test_index(self):
        result = index_check(1, 0)
        self.assertEqual(True, result)

    def test_second_index(self):
        result = index_check(5, 0)
        self.assertEqual(True, result)

    def test_third_index(self):
        result = index_check(5, 2)
        self.assertEqual(True, result)

    def test_multi_first(self):
        result = multiply(5, 5)
        self.assertEqual(25, result)

    def test_multi_second(self):
        result = multiply(5, 20)
        self.assertEqual(100, result)

    def test_divide(self):
        result = divide(20, 4)
        self.assertEqual(5, result)

    def test_abs(self):
        result = absolute(5)
        self.assertEqual(5, result)

    def test_abs_second(self):
        result = absolute(-5)
        self.assertEqual(5, result)

    def test_negative_divide(self):
        result = divide(-20, 4)
        self.assertEqual(-5, result)

    def test_double_negative_divide(self):
        result = divide(-20, -4)
        self.assertEqual(5, result)

    def test_power(self):
        result = power(2, 4)
        self.assertEqual(16, result)

    def test_power2(self):
        result = power(3, 3)
        self.assertEqual(27, result)

    def test_power3(self):
        result = power(4, 6)
        self.assertEqual(4096, result)

    def test_power4(self):
        result = power(2, 0)
        self.assertEqual(1, result)

    def test_power5(self):
        result = power(2, 1)
        self.assertEqual(2, result)

    def test_sqrt(self):
        result = square_root(9)
        self.assertEqual(3, result)

    def test_sqrt_second(self):
        result = square_root(25)
        self.assertEqual(5, result)


if __name__ == '__main__':
    unittest.main()
