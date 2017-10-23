.PHONY: all clean fclean re start

all:
	@xbuild "/p:Configuration=Release" "/target:build"

start: all
	@cd 42run/bin/Release && mono 42run.exe

clean:
	@xbuild "/p:Configuration=Release" "/target:clean"

fclean: clean

re:
	@xbuild "/p:Configuration=Release" "/target:clean;build"