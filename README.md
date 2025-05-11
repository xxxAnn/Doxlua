### Bytecode

Execute Mode

`GetGlobal FROMINDEX X X`
Insert Global at FROMINDEX index onto stack
GetGlobal        

`LoadConst FROMINDEX X X`
Insert Constant at FROMINDEX index onto stack
LoadConst

`Call ARGCOUNT RETURNCOUNT X`
Call function at with ARGCOUNT arguments from the stack 
Call


`LoadEnv X X X`
Load the environment table onto the stack
LoadEnv


Write Mode

`Write X X X`
Writes the string value atop the stack
`STACKINDEX = {`
OpenBlock

`Write X X X`
Writes the end of the block
`}`
CloseBlock

`Write X X X`
Writes the pair of the two elements atop the stack
`STACKINDEX1 = `STACKINDEX2`
Pair

`Write X X X`
Writes the element atop the stack
`STACKINDEX`
Element

### Globals

`TableAccess TABLE KEY`
Access KEY of the table 