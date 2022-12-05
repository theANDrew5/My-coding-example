!function () {
    "use strict";
    function e(e, t) {
        !function a() {
            var r = document.getElementById(e);
            if (r)
                return t(r);
            setTimeout(a, 100)
        }()
    }
    window.SafeRouteCartWidget = function (t, a, r, n) {
        console.log(typeof a);
        if (!t)
            return console.error("SafeRouteCartWidget: Argument 'id' is required.");
        if ("object" != typeof a)
            return console.error("SafeRouteCartWidget: Argument 'params' is required and must be an object.");
        if (void 0 === n ? n = "https://ddelivery.ru" : "" !== n && "/" !== n || (n = location.protocol + "//" + location.hostname),
            a.lang && window.messages[a.lang] || (a.lang = "ru"),
            a.apiScript || (a.apiScript = "/saferoute-widget-api.php"),
            -1 === a.apiScript.search(/^https?:\/\//) && (a.apiScript = location.protocol + "//" + location.hostname + a.apiScript),
            document.all && !window.atob)
            return alert(window.messages[a.lang].ieNotSupported);
        a.products && Array.isArray(a.products) || (a.products = []);
        var i = {
            start: null,
            beforeSubmit: null,
            afterSubmit: null,
            change: null,
            back: null,
            error: null
        };
        function o(e, t, a) {
            setTimeout(function () {
                var a = i[e];
                a && "function" == typeof a && a(t)
            }, a ? 0 : 125)
        }
        function s(e, a) {
            window.frames[t].postMessage && window.frames[t].postMessage({
                type: e,
                data: a
            }, "*")
        }
        function d(e) {
            var a = e.data;
            "object" == typeof a && a.type && a.id === t && ("change" === a.type ? o("change", a.data) : "event" === a.type ? "start" === a.data.event ? o(a.data.event) : "beforeSubmit" === a.data.event ? o(a.data.event, a.data.values, !0) : "back" === a.data.event ? o(a.data.event, void 0, !0) : "afterSubmit" === a.data.event && (o(a.data.event, a.data.response),
                "error" === a.data.response.status && o("error", [a.data.response.message])) : "error" === a.type && o("error", [a.data]))
        }
        this.on = function (e, t) {
            i[e] = t
        }
            ,
            this.submit = function () {
                s("action", {
                    action: "submit"
                })
            }
            ,
            this.destruct = function () {
                window.removeEventListener("message", d),
                    e(t, function (e) {
                        e.innerHTML = ""
                    })
            }
            ,
            function (t, a, r) {
                var i, u;
                i = a,
                    u = {},
                    Object.keys(i).map(function (e) {
                        var t = i[e];
                        "string" == typeof t && (t = t.trim()),
                            "discount" === e && void 0 !== t ? t = Math.round(t) || void 0 : "products" === e ? t = t.map(function (e) {
                                return e.nds = parseInt(e.nds),
                                    0 !== e.nds && 10 !== e.nds && 18 !== e.nds && (e.nds = 0),
                                    e.name && (e.name = e.name.toString().trim()),
                                    e.vendorCode && (e.vendorCode = e.vendorCode.toString().trim()),
                                    e.barcode && (e.barcode = e.barcode.toString().trim()),
                                    e.price = parseFloat(e.price),
                                    e.discount = Math.round(e.discount) || 0,
                                    e.count = parseInt(e.count),
                                    e
                            }) : "itemCount" === e && void 0 !== t ? t = parseInt(t) || void 0 : "priceDeclared" !== e && "weight" !== e || void 0 === t ? "width" !== e && "height" !== e && "length" !== e || void 0 === t || (t = Math.round(parseFloat(t)) || void 0) : t = parseFloat(t) || void 0,
                            u[e] = t
                    });
                var c, m, p, l, v, h, g, f, w, I = (c = a = u,
                    m = [],
                    Object.keys(c).map(function (e) {
                        if (window.validators[e]) {
                            var t = window.validators[e](c[e], c.lang);
                            !0 !== t && m.push(t)
                        }
                    }),
                    m);
                I.length ? o("error", I) : (p = n,
                    l = a.apiScript,
                    v = function (i) {
                        if ("ok" === i.status) {
                            var u = i.data.max_width ? i.data.max_width + "px" : "100%"
                                , c = i.data.height ? i.data.height + "px" : "400px";
                            e(t, function (e) {
                                var o = a.apiScript + (-1 === a.apiScript.search(/\?/) ? "?" : "&") + "url=" + n + "/widgets/cart/widget.html";
                                e.innerHTML = '<iframe src="' + o + '" style="width:100%;border:1px solid #cfcfcf;background:#fff;max-width:' + u + ";height:" + c + ';" name="' + t + '" frameborder="0"></iframe>',
                                    document.querySelector('iframe[name="' + t + '"]').onload = function () {
                                        window.addEventListener("message", d),
                                            setTimeout(function () {
                                                s("params", {
                                                    id: t,
                                                    params: a,
                                                    sdkSettings: i.data,
                                                    demoMode: r,
                                                    apiHost: n
                                                })
                                            }, 1e3)
                                    }
                            })
                        } else
                            "error" === i.status && (1 === i.code ? o("error", [window.messages[a.lang].invalidApiKey]) : o("error", [window.messages[a.lang].requestError]))
                    }
                    ,
                    f = new XMLHttpRequest,
                    w = p + "/api/:key/sdk/widget-settings.json",
                    f.open("GET", l + (h = l,
                        (g = document.createElement("a")).href = h,
                        g.search ? "&" : "?") + "url=" + w, !0),
                    f.send(),
                    f.onreadystatechange = function () {
                        var response = f.responseText;
                        4 === f.readyState && (200 === f.status ? v(JSON.parse(response)) : console.error("Ошибка при обращении к API-скрипту или серверу DDelivery"))
                    }
                )
            }(t, a, r)
    }
}(),
    function () {
        "use strict";
        window.messages = {
            ru: {
                paramItemCountInvalid: "Параметр 'itemCount' должен быть числом от 1 до 99",
                paramDiscountInvalid: "Параметр 'discount' не может быть меньше 0",
                paramPriceDeclaredInvalid: "Параметр 'priceDeclared' не может быть меньше 0",
                paramWidthInvalid: "Параметр 'width' имеет некорректное значение (должно быть число от 0 до 999 999)",
                paramHeightInvalid: "Параметр 'height' имеет некорректное значение (должно быть число от 0 до 999 999)",
                paramLengthInvalid: "Параметр 'length' имеет некорректное значение (должно быть число от 0 до 999 999)",
                paramWeightInvalid: "Параметр 'weight' меньше 0",
                paramProductsEmpty: "Параметр 'products' должен быть массивом, содержащим хотя бы один товар",
                paramProductsPriceInvalid: "Параметр 'price' одного из товаров отсутствует или имеет некорректное значение (должно быть число от 0 до 999 999)",
                paramProductsCountInvalid: "Параметр 'count' одного из товаров отсутствует или имеет некорректное значение (должно быть число от 1 до 999)",
                paramProductsDiscountInvalid: "Параметр 'discount' одного из товаров меньше 0",
                invalidApiKey: "Некорректный API-ключ",
                requestError: "Ошибка при отправке запроса на сервер DDelivery",
                ieNotSupported: "Виджет DDelivery не поддерживает IE версии 9 и ниже"
            },
            en: {
                paramItemCountInvalid: "Parameter 'itemCount' must be a number from 1 to 99",
                paramDiscountInvalid: "Parameter 'products' can not be less than 0",
                paramPriceDeclaredInvalid: "Parameter 'priceDeclared' can not be less than 0",
                paramWidthInvalid: "Parameter 'width' has an incorrect value (must be a number from 0 to 999 999)",
                paramHeightInvalid: "Parameter 'height' has an incorrect value (must be a number from 0 to 999 999)",
                paramLengthInvalid: "Parameter 'length' has an incorrect value (must be a number from 0 to 999 999)",
                paramWeightInvalid: "Parameter 'weight' is less than 0",
                paramProductsEmpty: "Parameter 'products' must be an array and non-empty",
                paramProductsPriceInvalid: "Parameter 'price' one of the products is missing or has an incorrect value (must be a number from 0 to 999 999)",
                paramProductsCountInvalid: "Parameter 'count' one of the products is missing or has an incorrect value (must be a number from 1 to 999)",
                paramProductsDiscountInvalid: "Parameter 'discount' of one of the products is less than 0",
                invalidApiKey: "Invalid API-key",
                requestError: "Request Error",
                ieNotSupported: "DDelivery widget does not support IE version 9 or lower"
            },
            zh: {
                paramItemCountInvalid: "",
                paramDiscountInvalid: "",
                paramPriceDeclaredInvalid: "",
                paramWidthInvalid: "",
                paramHeightInvalid: "",
                paramLengthInvalid: "",
                paramWeightInvalid: "",
                paramProductsEmpty: "",
                paramProductsPriceInvalid: "",
                paramProductsCountInvalid: "",
                paramProductsDiscountInvalid: "",
                invalidApiKey: "",
                requestError: "",
                ieNotSupported: ""
            }
        }
    }(),
    function () {
        "use strict";
        window.validators = {
            itemCount: function (e, t) {
                return e >= 1 && e <= 99 || void 0 === e || window.messages[t].paramItemCountInvalid
            },
            discount: function (e, t) {
                return !(e && e < 0) || window.messages[t].paramDiscountInvalid
            },
            priceDeclared: function (e, t) {
                return !(e && e < 0) || window.messages[t].paramPriceDeclaredInvalid
            },
            width: function (e, t) {
                return !e || !(e < 0 || e > 999999) || window.messages[t].paramWidthInvalid
            },
            height: function (e, t) {
                return !e || !(e < 0 || e > 999999) || window.messages[t].paramHeightInvalid
            },
            length: function (e, t) {
                return !e || !(e < 0 || e > 999999) || window.messages[t].paramLengthInvalid
            },
            weight: function (e, t) {
                return !(e && e < 0) || window.messages[t].paramWeightInvalid
            },
            products: function (e, t) {
                if (!e.length)
                    return window.messages[t].paramProductsEmpty;
                var a = null;
                return e.forEach(function (e) {
                    !e.price && 0 !== e.price || e.price < 0 || e.price > 999999 ? a = window.messages[t].paramProductsPriceInvalid : !e.count || e.count < 1 || e.count > 999 ? a = window.messages[t].paramProductsCountInvalid : e.discount && e.discount < 0 && (a = window.messages[t].paramProductsDiscountInvalid)
                }),
                    a || !0
            }
        }
    }();