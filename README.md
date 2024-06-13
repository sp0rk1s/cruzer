# Cruzer
[![License: MIT](https://img.shields.io/badge/License-MIT-yellow.svg)](https://opensource.org/licenses/MIT)

Version: Pre-Alpha

## Contents
- [Concept](#Concept)
- [Environments](#Environments)
- [Networking](Networking)

## Concept
Cruzer will be a easy online shell environment you can access on any pc using a quick code.
Each server will have its own environment filled with services for people to access.
There will be a website eventually where you can easily connect to your favorite enviorments without any downloading.
```ansi
Windows PowerShell
Copyright (C) Microsoft Corporation. All rights reserved.

Install the latest PowerShell for new features and improvements! https://aka.ms/PSWindows

PS C:\Users\guest> cruzer connect cruzer.nu -a=9a21

cruzer.nu : 9a21 >>> environment

random.cxt		random extension 1
cxt.cxt			cruzer extensions manager
random2.cxt		random extension 2
nano.cxt		nano editor

cruzer.nu : 9a21 >>> _
```

## Networking

### Header
| Byte		| Type			| Description		|
| --------- | ------------- | ----------------- |
| 0-7		| UInt64		| Account ID		|
| 8-11		| Int32			| Packet Size		|
| 12-15		| UInt32		| Packet Number		|
| 16-19		| UInt32		| Packet Type		|
| 20		| Boolean		| Respond			|
| 21		| Boolean		| Ignore account	|
| 22-512	| Int16:Int64	| Attributes		|

### Attribute
| Byte		| Type			| Description		|
| --------- | ------------- | ----------------- |
| 0-1		| Int16			| Key				|
| 2-9		| Int64			| Value				|

### Body
Open for use