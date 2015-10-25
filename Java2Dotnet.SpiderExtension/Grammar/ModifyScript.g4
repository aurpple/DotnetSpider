grammar ModifyScript;
/*
 * Parser Rules
 */
import CommonLexerRules;

expr: 
	expr op=('*'|'/') expr		#MulDiv 
	|expr op=('+'|'-') expr		#AddSub
	|INT						#Int
	|STRING						#String
	|FLOAT						#Float
	|regex						#Regex_
	|append						#Append_
	|prefix						#Prefix_
	|'(' expr ')'				#Parens
	;
 
param: STRING|BOOLEAN|INT|FLOAT|expr;
regex: 'regex' '(' STRING ',' INT ')';
append: 'append' '(' STRING ')';
prefix: 'prefix' '(' STRING ')';
