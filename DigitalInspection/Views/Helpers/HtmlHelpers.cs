using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using System.Text;
using System.Web.Mvc;
using System.Web.Mvc.Ajax;
using System.Web.Mvc.Html;
using System.Web.Routing;

namespace DigitalInspection.Views.Helpers
{
	public static class HtmlHelpers
	{
		// https://stackoverflow.com/a/13923013/2831961
		public static RouteValueDictionary ConditionalDisable(bool shouldDisable, object htmlAttributes = null)
		{
			var dictionary = HtmlHelper.AnonymousObjectToHtmlAttributes(htmlAttributes);

			if (shouldDisable)
			{
				dictionary.Add("disabled", "disabled");
			}

			return dictionary;
		}

		// https://forums.asp.net/post/4517653.aspx
		public static MvcHtmlString RawActionLink(this AjaxHelper ajaxHelper, string linkText, string actionName, string controllerName, object routeValues, AjaxOptions ajaxOptions, object htmlAttributes)
		{
			var repID = Guid.NewGuid().ToString();
			var lnk = ajaxHelper.ActionLink(repID, actionName, controllerName, routeValues, ajaxOptions, htmlAttributes);
			return MvcHtmlString.Create(lnk.ToString().Replace(repID, linkText));
		}

		public static string Pluralizer(string noun)
		{
			return noun.EndsWith("s") ? noun + "'" : noun + "'s";
		}

		public static MvcHtmlString HiddenForClass<TModel, TProperty>(
			this HtmlHelper<TModel> helper,
			Expression<Func<TModel, IEnumerable<TProperty>>> expression)
		{

			var sb = new StringBuilder();

			string membername = expression.GetMemberName();
			TModel model = helper.ViewData.Model;
			IEnumerable<TProperty> list = expression.Compile()(model);

			var subType = typeof(TProperty);
			var memberProperties = subType.GetProperties(BindingFlags.Public | BindingFlags.Instance)
				.Where(x => x.GetSetMethod(false) != null && x.GetGetMethod(false) != null)
				.Select(x => new
				{
					MemberPropName = x.Name,
					ListItemPropGetter = (Func<TProperty, object>)(p => x.GetValue(p, null))
				}).ToList();

			var klass = list.ElementAt(0);
			foreach (var property in memberProperties)
			{
				sb.Append(
					helper.Hidden(
						string.Format("{0}.{1}", membername, property.MemberPropName),
						property.ListItemPropGetter(klass)
					)
				);
			}

			return new MvcHtmlString(sb.ToString());
		}

		/// <summary>
		/// Returns hiddens for every IEnumerable item, with it's all public writable properties, if allPublicProps set to true.
		/// </summary>
		public static MvcHtmlString HiddenForEnumerable<TModel, TModelProperty>(
			this HtmlHelper<TModel> helper,
			Expression<Func<TModel, IEnumerable<TModelProperty>>> expression,
			bool allPublicProps)
		{
			if (!allPublicProps)
			{
				return HiddenForEnumerable(helper, expression);
			}

			var sb = new StringBuilder();

			string membername = expression.GetMemberName();
			TModel model = helper.ViewData.Model;
			IEnumerable<TModelProperty> list = expression.Compile()(model);

			var subType = typeof(TModelProperty);
			var memberProperties = subType.GetProperties(BindingFlags.Public | BindingFlags.Instance)
				.Where(x => x.GetSetMethod(false) != null && x.GetGetMethod(false) != null)
				.Select(x => new
				{
					MemberPropName = x.Name,
					ListItemPropGetter = (Func<TModelProperty, object>)(p => x.GetValue(p, null))
				}).ToList();

			if (memberProperties.Count == 0)
			{
				return HiddenForEnumerable(helper, expression);
			}

			for (var i = 0; i < list.Count(); i++)
			{
				var listItem = list.ElementAt(i);
				foreach (var property in memberProperties)
				{
					sb.Append(
						helper.Hidden(
							string.Format("{0}[{1}].{2}", membername, i, property.MemberPropName),
							property.ListItemPropGetter(listItem)
						)
					);
				}
			}

			return new MvcHtmlString(sb.ToString());
		}

		// https://stackoverflow.com/a/35228850/2831961
		public static MvcHtmlString HiddenForEnumerable<TModel, TProperty>(
			this HtmlHelper<TModel> helper,
			Expression<Func<TModel, IEnumerable<TProperty>>> expression)
		{
			var sb = new StringBuilder();

			var membername = expression.GetMemberName();
			var model = helper.ViewData.Model;
			var list = expression.Compile()(model);

			for (var i = 0; i < list.Count(); i++)
			{
				sb.Append(helper.Hidden(string.Format("{0}[{1}]", membername, i), list.ElementAt(i)));
			}

			return new MvcHtmlString(sb.ToString());
		}

		public static string GetMemberName<TModel, T>(this Expression<Func<TModel, T>> input)
		{
			if (input == null)
			{
				return null;
			}

			MemberExpression memberExp = null;

			if (input.Body.NodeType == ExpressionType.MemberAccess)
			{
				memberExp = input.Body as MemberExpression;
			}
			else if (input.Body.NodeType == ExpressionType.Convert)
			{
				memberExp = ((UnaryExpression)input.Body).Operand as MemberExpression;
			}

			return memberExp?.Member?.Name;
		}
	}
}