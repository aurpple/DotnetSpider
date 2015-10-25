lexer grammar CommonLexerRules;

fragment DIGIT : [0-9];
NEWLINE: '\n'?;
BOOLEAN: ('true'|'false')+;
FLOAT: '-'?DIGIT+ '.' DIGIT*; // match 1. 39. 3.14159 etc...
INT: '-'?[0-9]+;
NUMBER: [0-9]+;
MUL : '*' ; // assigns token name to '*' used above in grammar
DIV : '/' ;
ADD : '+' ;
SUB : '-' ;
ESC : '\\"' | '\\\\' ; // 2-char sequences \" and \\
ID: [a-z0-9A-Z_]+;
STRING: '\'' (ESC|.)*? '\''  ;
WS : [ \t\r]+ -> skip ; // toss out whitespace
LINE_COMMENT : '//' .*? '\n' -> skip ;
COMMENT : '/*' .*? '*/' -> skip ;