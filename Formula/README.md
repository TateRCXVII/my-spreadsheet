```
Author:     Tate Reynolds
Partner:    None
Date:       4-Feb-2022
Course:     CS 3500, University of Utah, School of Computing
GitHub ID:  TateRCXVII
Repo:       https://github.com/Utah-School-of-Computing-de-St-Germain/spreadsheet-TateRCXVII/tree/master/Formula
Commit #:   ************************
Project:    Formula
Copyright:  CS 3500 and Tate Reynolds - This work may not be copied for use in Academic Coursework.
```

# Comments to Evaluators:
There is only one branch that isn't checked by tests, which is the reason for the 99%. Because I get nervous about removing exception handling,
I didn't delete the branch, even though I couldn't make a test case that would call that branch directly. I figured it'd be better to handle a rare 
exception than to delete the branch and have it break my code later.
# Time Estimate vs. Actual Breakdown
|        | Expected | Actual | Notes                                                                                                                    |
|--------|----------|--------|--------------------------------------------------------------------------------------------------------------------------|
|Analysis| 3        | 5     | My first time reading the assignment I thought things were more simple than they were. So, the analysis took me much longer.|
|Testing | 3        | 3      | I pushed myself to write tests first so I could really understand the assignment and how it would operate. This took a good chunk of time.                |
|Implementation | 4        | 2      | Once I understood how to implement the changes, things fell into place quite well.                                                                |
|Debug   | 2        | 2      | As I added more tests, more bugs were apparent.               |
| Total: | 12       | 12      | This assignment, despite my initial misconception, took about the same time I had expected. The time allotment just fell into different categories.  |

# Assignment Specific Topics
Funcs and Lambdas are very useful! Glad C# lets us pass functions as parameters.

# Consulted Peers:
I should branch out to include new people...
- Sanjay Gounder
- Thatcher Geary


# References:

    1. Double.TryParse() - https://stackoverflow.com/questions/6733652/how-can-i-check-if-a-string-is-a-number/6733665
    2. Double divide by 0 value - https://stackoverflow.com/questions/44258124/divide-by-zero-and-no-error/44258269
    3. Casting - https://stackoverflow.com/questions/18784274/converting-object-of-a-class-to-of-another-one
    4. Catch different exceptions - https://stackoverflow.com/questions/136035/catch-multiple-exceptions-at-once
    5. .NET Core .GetHashCode() - https://andrewlock.net/why-is-string-gethashcode-different-each-time-i-run-my-program-in-net-core/
    6. == vs Equals - https://stackoverflow.com/questions/814878/c-sharp-difference-between-and-equals