grammar ModifyScript;
/*
 * Parser Rules
 */
import CommonLexerRules;

stats: stat+;
stat: expr ';' 
	| ';'
	;
expr: 
	expr op=('*'|'/') expr		#MulDiv 
	|expr op=('+'|'-') expr		#AddSub
	|INT						#Int
	|STRING						#String
	|FLOAT						#Float
	|regex						#Regex_
	|append						#Append_
	|prefix						#Prefix_
	|replace					#Replace_
	|stoper						#Stoper_
	|'(' expr ')'				#Parens
	;
param: STRING|BOOLEAN|INT|FLOAT|expr;
compare: '>'|'<'|'=';
regex: 'regex' '(' STRING ',' INT ')';
append: 'append' '(' STRING ')';
prefix: 'prefix' '(' STRING ')';
replace: 'replace' '(' STRING ',' STRING ')';
stoper: 'stoper' '(' compare ',' STRING ')';
