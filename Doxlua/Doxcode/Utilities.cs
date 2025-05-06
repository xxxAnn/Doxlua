using Doxlua.VM;

namespace Doxlua.Doxcode
{
// Some common patterns:

// We have the statement: `print("Hello world")`
// Our const table is ["print", "Hello world"]
// GlobalCodes.TableAccess = 3

// LoadConst  1 0 0  ; Loads "Hello world" onto the stack
// LoadConst  0 0 0  ; Loads "print" onto the stack
// LoadEnv    0 0 0  ; Loads _GLOBALS_ onto the stack
// GetGlobal  3 0 0  ; Puts TableAccess on the stack
// ======= Stack =======
// |     TableAccess   ; Function
// |     _GLOBALS_     ; Global DoxTable
// |     "print"       ; DoxString
// |     "Hello world" ; DoxString
// ======================
// ; Call TableAccess with _GLOBALS_ and "print" 
// ; The Executor consumes the top element of the stack
// ; The Function consumes the two following elements
// Call       2 1 0 
// 
// ======= Stack =======
// |     print         ; Function
// |     "Hello world" ; DoxString
// =======================
// Call       1 0 0 ; Prints "Hello world"
// ======= Stack =======
// |                    ; Empty
// =====================
//
// --------------------------------------------------
//
// When Loading a function we want to roughly:
// Call DOX_SETTABLE(ENV, "ARG_NAME1", value),
// Call DOX_SETTABLE(ENV, "ARG_NAME2", value),
// Call DOX_SETTABLE(ENV, "ARG_NAME3", value),
// ...
// Since all the values are in reverse order loaded on the stack we go in reverse order
//
// Suppose:
// Last argument's const index is 12
// Second argument's const index is 11
// First argument's const index is 10
// DOX_SETTABLE's const index is 123
//
// LoadConst  12 0 0 ; Loads "ARG_NAME1" onto the stack
// LoadEnv     0 0 0 ; Loads _GLOBALS_ onto the stack
// LoadConst 123 0 0 ; Loads "DOX_SETTABLE" onto the stack
// Utilities.GetGlobal()
// ======= Stack =======
// |     DOX_SETTABLE      ; Function
// |     _GLOBALS_         ; Global DoxTable
// |     "ARG_NAME1"       ; DoxString
// |     argument_value3   ; Any DoxValue
// |     argument_value2   ; Any DoxValue
// |     argument_value1   ; Any DoxValue
// ======================
// Call        3 0 0       ; Call DOX_SETTABLE with _GLOBALS_, "ARG_NAME" and argument_value1
// Repeat for Arguments 2 and 3

    public static class Utilities 
    {
        // Adds the env function to the top of the stack
        // Top of the stack is the env function name
        public static byte[][] GetGlobal() =>
            [
                Doxcode.Bytecode.ToByteArray(
                    Doxcode.BytecodeMode.Execute,
                    Doxcode.BytecodeOp.LoadEnv,
                    0, 0, 0
                ),
                // Stack is now: _GLOBALS_(Table), FuncName(String)
                Doxcode.Bytecode.ToByteArray(
                    Doxcode.BytecodeMode.Execute,
                    Doxcode.BytecodeOp.GetGlobal,
                    GlobalCodes.TableAccess, 0, 0
                ),
                // Stack is now: TableAccess(Func), _Globals_(Table), FuncName(String)
                Doxcode.Bytecode.ToByteArray(
                    Doxcode.BytecodeMode.Execute,
                    Doxcode.BytecodeOp.Call,
                    2, 1, 0
                ),
                // Stack is now: Print(Func)

            ];
    }
}