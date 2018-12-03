define(function (require, exports, module) {
    var parentNotice = {};
    parentNotice.msgPanel = function () {
        if (IsLogin) {
            if (CurrentUser.Type == 3) {//家长通知ajax判断
                AjaxPost("/Parent/CheckNewReply", {}, function (jsonData) {
                    if (jsonData.Result > 0) {
                        $(".icon-sidebar-11").next("em").show();
                        $("#parent_careerQA_point").show();
                    } else {
                        //$(".icon-sidebar-11").next("em").hide();
                        $("#parent_careerQA_point").hide();
                        AjaxPost("/Parent/CheckNewCareerNotify", {}, function (jsonData) {
                            if (jsonData.Result > 0) {
                                $(".icon-sidebar-11").next("em").show();
                                $("#parent_careerNotify_point").show();
                            } else {
                                $(".icon-sidebar-11").next("em").hide();
                                $("#parent_careerNotify_point").hide();

                            }
                        })
                    }
                })                
            } else if (CurrentUser.Type == 2) {//导师通知ajax判断
                AjaxPost("/Teacher/CheckNewCareerQA", {}, function (jsonData) {
                    if (jsonData.Result > 0) {
                        $(".icon-sidebar-16").next("em").show();
                        $("#teacher_careerQA_point").show();  
                    } else {
                        $(".icon-sidebar-16").next("em").hide();
                        $("#teacher_careerQA_point").hide();
                    }
                });
                AjaxPost("/Teacher/CheckNewStudentTeacherQA", {}, function (jsonData) {
                    if (jsonData.Result > 0) {
                        $(".icon-sidebar-17").next("em").show();
                        $("#teacher_tutorQA_point").show();  
                    } else {
                        $(".icon-sidebar-17").next("em").hide();
                        $("#teacher_tutorQA_point").hide();
                    }
                });
            }else if(CurrentUser.Type==1){//学生通知ajax判断
                AjaxPost("/Student/GetTutorNotAnswerReadCount", {}, function (jsonData) {
                    if (jsonData.Result > 0) {
                        $(".icon-sidebar-17").next("em").show();
                        $("#student_tutorQA_point").show();  
                    } else {
                        $(".icon-sidebar-17").next("em").hide();
                        $("#student_tutorQA_point").hide();
                    }
                });
            }
        }
    }
    parentNotice.doOpt = function () {
        //this.msgPanel();
    };
    module.exports = parentNotice;
})