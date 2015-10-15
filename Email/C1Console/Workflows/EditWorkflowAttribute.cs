using System;

namespace CompositeC1Contrib.Email.C1Console.Workflows
{
    [AttributeUsage(AttributeTargets.Class)]
    public class EditWorkflowAttribute : Attribute
    {
        public Type EditWorkflowType { get; set; }

        public EditWorkflowAttribute(Type type)
        {
            EditWorkflowType = type;
        }
    }
}
