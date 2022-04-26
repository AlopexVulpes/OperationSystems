import time

b, c = 3, 3
a = 0

timer1 = time.perf_counter()

for i in range(100000000):
    a += b*2 + c - i

timer2 = time.perf_counter()

print('The result of the function execution:', a)
print('Time taken to execute:', timer2 - timer1)
