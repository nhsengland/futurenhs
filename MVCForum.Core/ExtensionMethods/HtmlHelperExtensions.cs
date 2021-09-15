namespace MvcForum.Core.ExtensionMethods
{
    using System;
    using System.Linq.Expressions;
    using System.Text;
    using System.Web.Mvc;

    public static class HttpHelperExtensions
    {
        public static string FieldHasError(this HtmlHelper helper, string propertyName, string errorClass = "text-danger")
        {
            if (helper.ViewData.ModelState != null && !helper.ViewData.ModelState.IsValidField(propertyName))
            {
                return errorClass;
            }
            return string.Empty;
        }

        public static string FieldHasError<TModel, TEnum>(this HtmlHelper<TModel> helper, Expression<Func<TModel, TEnum>> expression, string errorClass = "has-danger")
        {
            var expressionString = ExpressionHelper.GetExpressionText(expression);
            var modelName = helper.ViewContext.ViewData.TemplateInfo.GetFullHtmlFieldName(expressionString);
            return FieldHasError(helper, modelName, errorClass);
        }

        public static string CustomValidationSummary(this HtmlHelper helper, string errorClass = "")
        {
            if (helper.ViewData.ModelState.IsValid)
                return string.Empty;

            StringBuilder sb = new StringBuilder();

            foreach (var key in helper.ViewData.ModelState.Keys)
            {
                foreach (var err in helper.ViewData.ModelState[key].Errors) {

                    var rawHtmlErrMsg = err.ErrorMessage;
                    var encodedErrMsg = helper.Encode(err.ErrorMessage);

                    if (!(helper.ViewData.ModelState[key].Value is null))
                    {
                        sb.Append($"<li> <a class='{errorClass}' href='#{key}'>{rawHtmlErrMsg}</a></li>");
                    } 
                    else
                    {
                        sb.Append($"<li class='{errorClass}'>{rawHtmlErrMsg}</li>");
                    }                    
                }
            }
            return sb.ToString();
        }
    }
}