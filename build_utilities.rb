class Compiler
	def self.compile(attributes)
		version = attributes.fetch(:clrversion, 'v3.5')
		compile_target = attributes.fetch(:compile_target, 'debug')
		solution_file = attributes[:solution_file]

		framework_dir = File.join(ENV['windir'].dup, 'Microsoft.NET', 'Framework', 'v3.5')
		msbuild_file = File.join(framework_dir, 'MSBuild.exe')

		sh "#{msbuild_file} #{solution_file} /property:Configuration=#{compile_target} /t:Rebuild"
	end
end
