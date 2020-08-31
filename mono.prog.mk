# Template for building Mono C# applications on OpenBSD

PROPS?=
DOCFILE?=
SRCDIR?=	src
BINDIR?=	bin

# Use major version 7.0 as a default
LANG?=	7.0

# Ensure required variables are set
.poison empty (PROG)
.poison empty (SRCS)
.poison empty (MAIN)
.poison empty (SRCDIR)
.poison empty (BINDIR)

# Set compiler flags
.ifmake debug
OUT=	${BINDIR}/Debug/${PROG}.exe
.else
OUT=	${BINDIR}/${PROG}.exe
.endif

FLAGS=	-nologo -main:${MAIN} -out:${OUT}
FLAGS+=	-langversion:\"${LANG}\"
.if defined(APPCONFIG)
FLAGS+=	-appconfig:${APPCONFIG}
.endif
.if defined(CSSYMBOL)
FLAGS+=	-define:${CSSYMBOL}
.endif
# Add in libraries
.if defined(LIBS)
.  ifmake debug
FLAGS+=	-lib:${BINDIR}/Debug
.  else
FLAGS+=	-lib:${BINDIR}
.  endif
.  for l in ${LIBS}
FLAGS+=	-reference:${l}.dll
.  endfor
.endif
DFLAGS=	-debug -define:DEBUG -optimize-

# Real location of the C# source files
CS=	${SRCS:S,^,$(SRCDIR)/,g}

all release: testobj libs
	csc ${FLAGS} ${CS} ${PROPS}
	@echo '  ${PROG} -> ${.CURDIR}/${OUT}'

debug: testobj libs
	csc ${FLAGS} ${DFLAGS} ${CS} ${PROPS}
	@echo '  ${PROG} -> ${.CURDIR}/${OUT}'

doc: testobj
.if !empty(DOCFILE)
	csc ${FLAGS} -doc:${DOCFILE} ${CS}
	@echo '  ${PROG} -> ${.CURDIR}/${OUT}'
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

libs:
.if defined(LIBS)
	@echo '===> building libs'
.  ifmake debug
	cd ${.CURDIR}/lib && make obj && make CSSYMBOL2=${CSSYMBOL} debug
.    for l in ${LIBS}
	@cp ${.CURDIR}/lib/bin/Debug/${l}{.dll,.pdb} ${BINDIR}/Debug
.    endfor
.  else
	cd ${.CURDIR}/lib && make obj && make CSSYMBOL2=${CSSYMBOL}
.    for l in ${LIBS}
	@cp ${.CURDIR}/lib/bin/${l}.dll ${BINDIR}
.    endfor
.  endif
	@echo '===> done with libs'
.endif

clean:
	rm -rf ${BINDIR}
	rm -f ${PROG}.exe ${DOCFILE}
	@mkdir -p ${BINDIR}/Debug
