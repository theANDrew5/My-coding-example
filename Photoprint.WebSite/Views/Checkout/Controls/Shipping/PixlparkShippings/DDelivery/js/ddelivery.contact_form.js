/**
 * Created by DnAp on 11.04.14.
 */
var ContactForm = (function () {
    var el;

    return {
        init: function () {
            $('.phone-mask').mask("+7(999)999-99-99");

            var form = $('#main_form');
            form.submit(function () {return false;});

            function validateEmail(email){
                var re = /\S+@\S+\.\S+/;
                return re.test(email);
            };

            $('input', form).on('required', function() {
                var el = $(this), val;
                if(el.hasClass('tipped')){
                    val = '';
                }else{
                    val = el.val().trim();
                }
                if(el.attr('req') && val.length == 0) {
                    el.closest('.row__inp').addClass('error');
                }else{
                    el.closest('.row__inp').removeClass('error');
                }
                if( el.attr('id') == 'email' && el.val() != '@email.ru' ){
                    if( !validateEmail( el.val() ) ){
                        el.closest('.row__inp').addClass('error');
                    }
                }
            }).blur(function(){
                $(this).trigger('required');
                return true;
            }).on('keyup', function(){
                $(this).trigger('required');
            });

            $('.row-btns a.next').click(function () {
            	$('input, textarea', form).trigger('required');
                if($('.error', form).length > 0){
                    return false;
                }
                contact_form = form.serializeArray();
                DDeliveryIframe.ajaxPage({ contact_form: contact_form, action: 'change', city_id: $('#cityId').val() });
            });

            $('.row-btns a.prev').click(function () {
            	DDeliveryIframe.ajaxPage({ type: $(this).data('type'), action: "init", city_id: $('#cityId').val() });
            });

            $('input[title], textarea[title]', form).formtips();
        }
    }
})();
