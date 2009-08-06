require 'fileutils'
require 'build_utilities.rb'

COMPILE_TARGET = 'debug'

task :default => :build

task :build => :test

desc 'Compiles the Ottoman project'
task :compile do
	#TODO:  Add a central location to set the value of our project name
	Compiler.compile :compile_target => COMPILE_TARGET, :solution_file => 'src/SineSignal.Ottoman.sln'
end

desc 'Run tests for the Ottoman project'
task :test => :compile do
	runner = Tester.new :compile_target => COMPILE_TARGET, :show_report => false
	#TODO:  Add a central location to set the value of our test dll
	runner.run "SineSignal.Ottoman.Tests"
end
