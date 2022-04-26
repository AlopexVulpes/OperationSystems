#include <iostream>
#include <chrono>
#include <ctime>
#include <cmath>

using namespace std;

class Timer
{
public:
    void start()
    {
        m_StartTime = std::chrono::system_clock::now();
        m_bRunning = true;
    }

    void stop()
    {
        m_EndTime = std::chrono::system_clock::now();
        m_bRunning = false;
    }

    double elapsedMilliseconds()
    {
        std::chrono::time_point<std::chrono::system_clock> endTime;

        if (m_bRunning)
        {
            endTime = std::chrono::system_clock::now();
        }
        else
        {
            endTime = m_EndTime;
        }

        return std::chrono::duration_cast<std::chrono::milliseconds>(endTime - m_StartTime).count();
    }

    double elapsedSeconds()
    {
        return elapsedMilliseconds() / 1000.0;
    }

private:
    std::chrono::time_point<std::chrono::system_clock> m_StartTime;
    std::chrono::time_point<std::chrono::system_clock> m_EndTime;
    bool                                               m_bRunning = false;
};

long fibonacci(unsigned n)
{
    if (n < 2) return n;
    return fibonacci(n - 1) + fibonacci(n - 2);
}

int main() {
    Timer timer;
    long long a = 0;
    long long b = 3;
    long long c = 3;

    timer.start();

    for (long long i = 0; i < 100000000; i++) {
        a += b * 2 + c - i;
    }

    timer.stop();

    cout << "The result of the function execution: " << a << endl;
    std::cout << "Time taken to execute: " << timer.elapsedSeconds() << endl;
}
