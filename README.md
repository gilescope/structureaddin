structureaddin
==============

A Microsoft Project Add-In to import from JIRA's Structure Plugin

StructureAddin is an oppinionated way of laying out a structure such that it can be exported as an MPP.

The rules are:

	1. A parent is considered complete once all children are complete.

	2. A child has an implicit dependency on it's previous sibling (or if it is first, the parents previous sibling)
	   unless the child's summary starts with '*'.

	3. Siblings with '||' at the start of the summary can be done in parallel (assuming different assignee).
	
	4. Assignees, Remaining estimates and Due Dates are respected.

Project Goals:

By design the import is one-way from JIRA. This forces there to be a single golden source of up-to-date 
project information. The aim of this project is to remove the disconnect / reconsiliation of the high level project 
plan (managed upwards) with what's actually happening on the ground in JIRA.