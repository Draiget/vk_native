﻿using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.Text;

namespace vk.gen;

internal class VulkanSpecCodeWriter
{
    private readonly GeneratorExecutionContext _context;
    private readonly StringBuilder _sb;
    private int _indent;

    public VulkanSpecCodeWriter(GeneratorExecutionContext context) {
        _context = context;
        _indent = 0;
        _sb = new StringBuilder();
    }

    public void FinalizeSourceFile(string name) {
        _context.AddSource($"{name}.Gen", SourceText.From(_sb.ToString(), Encoding.UTF8));
        _indent = 0;
        _sb.Clear();
    }

    public void BeginBlock() {
        _indent++;
    }

    public void EndBlock() {
        _indent--;
    }

    public void UseDefaultHeaderComment() {
        WriteLine("// This file is automatically generated, do not edit");
    }

    public void UseSingleComment(string text) {
        WriteLine($"// {text}");
    }

    public void BeginComment() {
        WriteLine("/// <summary>");
    }

    public void UseMultiComment(string text) {
        WriteLine($"/// {text}");
    }

    public void EndComment() {
        WriteLine("/// </summary>");
    }

    public void UseNamespace(string ns) {
        WriteLine($"namespace {ns};\n");
    }

    public void WriteLine(string text) {
        UseIndent();
        _sb.Append($"{text}\n");
    }

    public void Write(string text) {
        _sb.Append($"{text}");
    }

    private void UseIndent() {
        if (_indent < 1) {
            return;
        }

        _sb.Append(new string('\t', _indent));
    }
}
