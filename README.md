### Bytecode

Execute Mode

`GetGlobal FROMINDEX X X`<br>
Insert Global at FROMINDEX index onto stack<br>
GetGlobal        

`LoadConst FROMINDEX X X`<br>
Insert Constant at FROMINDEX index onto stack<br>
LoadConst

`Call ARGCOUNT RETURNCOUNT X`<br>
Call function at with ARGCOUNT arguments from the stack <br>
Call


`LoadEnv X X X`<br>
Load the environment table onto the stack<br>
LoadEnv


Write Mode

`Write X X X`<br>
Writes the string value atop the stack<br>
`STACKINDEX = {`<br>
OpenBlock

`Write X X X`<br>
Writes the end of the block<br>
`}`<br>
CloseBlock

`Write X X X`<br>
Writes the pair of the two elements atop the stack<br>
`STACKINDEX1 = STACKINDEX2`<br>
Pair

`Write X X X`<br>
Writes the element atop the stack<br>
`STACKINDEX`<br>
Element

### Globals

`TableAccess TABLE KEY` (3)
Access KEY of the table 

`SetValue KEY VALUE` (5)
Set KEY to VALUE