require 'fileutils'
require 'build_utilities.rb'

COMPILE_TARGET = 'debug'

task :default => :build

task :build => :compile

desc 'Compiles the Ottoman project'
task :compile do
	Compiler.compile :compile_target => COMPILE_TARGET, :solution_file => 'src\\SineSignal.Ottoman.sln'
end
