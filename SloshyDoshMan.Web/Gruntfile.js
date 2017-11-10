'use strict';
var path = require('path');

module.exports = function(grunt) {
	grunt.initConfig({
		project: {
			out: './dist',
			build: './build',
			src: './src'
		},
		ts: {
			options: {
				target: 'es6',
				module: 'commonjs',
				inlineSourceMap: true,
				declation: false,
				alwaysStrict: true,
				strict: true,
				forceConsistentCasingInFileNames: true,
				noFallthroughCaseInSwitch: true,
				noImplicitAny: true,
				noImplicitReturns: true,
				noImplicitThis: true,
				noUnusedLocals: true,
				pretty: false,
				fast: 'never',
				baseUrl: '<%= project.src %>'
			},
			default : {
				src: ['<%= project.src %>/**/*.ts'],
				outDir: '<%= project.build %>'
			}
		},
		webpack: {
			options: {
				progress: false
			},
			prod: {
				entry: './<%=project.build %>/App.js',
				output: {
					path: path.resolve('<%=project.out %>'),
					filename: 'manifest.js'
				},
				resolve: {
					modules: [path.resolve('<%= project.build %>'), "node_modules"]
				},
				externals: {
					'jquery': 'jQuery',
					'moment': 'moment',
					'knockout': 'ko'
				}
			}
		},
		sass: {
			dist: {
				options: {
					style: 'expanded'
				},
				files: { 
					'<%=project.out%>/manifest.css': '<%=project.src%>/manifest.scss'
				}
			}
		},
		copy: {
			serverImages: {
				cwd: 'src/ServerImages',
				src: ['./*'],
				dest: '<%=project.out%>/CommonImages/server-images',
				expand: true
			},
			html: {
				cwd: 'src/',
				src: ['./*.html'],
				dest: '<%=project.out%>/',
				expand: true
			},
			images: {
				cwd: 'src/',
				src: ['./favicon.ico', './robots.txt', './CommonImages/**/*.{jpg,png}', './Perks/**/*.{jpg,png}', './Maps/**/*.{jpg,png}'],
				dest: '<%=project.out%>/',
				expand: true
			}
		},
		clean: {
			out: ['<%= project.out %>'],
			build: ['<%= project.build %>']
		},
		watch: {
			styles: {
				files: ['src/**/*.scss'],
				tasks: ['sass'],
				options: {
					spawn: false
				}
			},
			webscripts: {
				files: ['src/**/*.ts'],
				tasks: ['ts', 'webpack:prod']
			},
			images: {
				files: ['src/**/*.jpg', 'src/**/*.png'],
				tasks: ['copy:images']
			},
			html: {
				files: ['src/**/*.html'],
				tasks: ['copy:html']
			}
		}
	});

	grunt.loadNpmTasks('grunt-contrib-copy');
	grunt.loadNpmTasks('grunt-contrib-clean');
	grunt.loadNpmTasks('grunt-contrib-watch');
	grunt.loadNpmTasks('grunt-sass');
	grunt.loadNpmTasks('grunt-browserify');
	grunt.loadNpmTasks('grunt-ts');
	grunt.loadNpmTasks('grunt-webpack');

	grunt.registerTask('default', ['clean:build','ts','clean:out','sass','copy','webpack:prod']);
};