function main()
    print("Hello, World!")
end

-- Comment
main()

test("test", function()
    assert(main() == nil)
end)

--[[
    This is a multi-line comment
    that spans multiple lines.
--]]

-- defines a factorial function
function fact (n)
    if n == 0 then
        return 1
    else
        return n * fact(n-1)
    end
    end
    
    print("enter a number:")
    a = io.read("*number")        -- read a number
    print(fact(a))

t = { ["a"] = 1, ["b"] = 2, ["c"] = 3 }


print(t.a) -- prints 1

-- some more tests

a = {}     -- create a table and store its reference in `a'
k = "x"
a[k] = 10        -- new entry, with key="x" and value=10
a[20] = "great"  -- new entry, with key=20 and value="great"
print(a["x"])    --> 10
k = 20
print(a[k])      --> "great"
a["x"] = a["x"] + 1     -- increments entry "x"
print(a["x"])    --> 11

-- more tests 

i = 10; j = "10"; k = "+10"
a = {}
a[i] = "one value"
a[j] = "another value"
a[k] = "yet another value"
print(a[j])            --> another value
print(a[k])            --> yet another value
print(a[tonumber(j)])  --> one value
print(a[tonumber(k)])  --> one value