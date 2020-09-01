# Template for building Mono C# libs on OpenBSD

.poison empty (LIB)

SRCDIR?=	${LIB}
BINDIR?=	bin

# Use major version 7.0 as a default
LANG?=	7.0

# Ensure required variables are set
.poison empty (SRCS)
.poison empty (SRCDIR)
.poison empty (BINDIR)

# Set compiler flags
.ifmake debug
OUT=	${BINDIR}/Debug/${LIB}.dll
.else
OUT=	${BINDIR}/${LIB}.dll
.endif

FLAGS=	-nologo -target:library -out:${OUT}
FLAGS+=	-langversion:\"${LANG}\"
.if defined(CSSYMBOL)
FLAGS+= -define:${CSSYMBOL}
.endif
# This is only used for passing symbol(s) from the 'main' app to the libraries
.if !empty(CSSYMBOL2)
FLAGS+= -define:${CSSYMBOL2}
.endif
DFLAGS=	-debug -define:DEBUG -optimize-

# Real location of the C# source files
CS=	${SRCS:S,^,$(SRCDIR)/,g}

all release: testobj
	csc ${FLAGS} ${CS}
	@echo '  ${LIB} -> ${.CURDIR}/${OUT}'

debug: testobj
	csc ${FLAGS} ${DFLAGS} ${CS}
	@echo '  ${LIB} -> ${.CURDIR}/${OUT}'

doc: testobj
.if defined(DOCFILE)
	csc ${FLAGS} -doc:${DOCFILE} ${CS}
	@echo '  ${LIB} -> ${.CURDIR}/${OUT}'
.else
	@echo 'No DOCFILE defined, nothing to do'
.endif

testobj:
.if !exists(${BINDIR})
	@echo 'No output directory! Run `make obj` before proceeding'
	@false
.endif

obj:
.if !exists(${BINDIR})
	mkdir -p ${BINDIR}/Debug
.endif

clean:
	rm -rf ${BINDIR}
	rm -f ${LIB}.dll ${DOCFILE}
	@mkdir -p ${BINDIR}/Debug
