using System;
using System.Collections.Generic;
using System.Text;
using System.Net.Mail;
using System.ServiceProcess;
using System.Runtime.InteropServices;

/*
DLL 안의 SendMailBySmtp 함수를 호출하면 원인을 addresses가 empty string일 수 없다는
에러가 발생하는 데 원인을 찾을 수 없음.
그래서 다음과 같이 사용함.
MailMessage mm = new MailMessage(From, To, Subject, Body);
mm.IsBodyHtml = true;
SmtpClient sc = new SmtpClient("mail.sitename.com");
sc.Send(mm);
 */
namespace DoctorGu
{
	/// <summary>
	/// 이메일과 관련된 기능 구현
	/// </summary>
	public class CEmail
	{
		//private bool mHasSmtpService = false;
		//private ServiceController mSmtpServ = null;
		
		public CEmail()
		{
			//ServiceController[] services = ServiceController.GetServices();

			//// Loop through all the services on the machine and find the SMTP Service.
			//foreach (ServiceController service in services)
			//{
			//	if (service.ServiceName.ToLower() == "smtpsvc")
			//	{
			//		this.mHasSmtpService = true;
			//		this.mSmtpServ = service;
			//		break;
			//	}

			//}
		}

		/// <summary>
		/// SMTP 서버를 이용해 이메일을 전송함.
		/// </summary>
		/// <param name="SmtpServer">SMTP 서버명(윈도우에 포함된 기본 SMTP 서버를 쓸 경우 일반적으로 컴퓨터 이름이 됨)</param>
		/// <param name="From">보내는 메일 주소</param>
		/// <param name="To">받는 메일 주소</param>
		/// <param name="Cc">참조 메일 주소</param>
		/// <param name="Bcc">숨은 참조 메일 주소</param>
		/// <param name="ReplyTo">회신 메일 주소</param>
		/// <param name="Subject">메일 제목</param>
		/// <param name="Body">메일 내용</param>
		/// <param name="Priority">메일 중요도</param>
		/// <param name="aAttachments">메일 첨부 파일</param>
		/// <example>
		/// <code>
		/// <![CDATA[
		/// CEmail em = new CEmail();
		/// em.SendMailBySmtp(Environment.MachineName, "운영자<doctorgu@naver.com>", "doctorgu@naver.com", "", "", "",
		///	 "테스트 제목", "테스트 내용", MailPriority.High,
		///	 new Attachment[] { new Attachment(@"C:\temp\serverquit.doc"), new Attachment(@"C:\Test.log") });
		/// ]]>
		/// </code>
		/// </example>
		public void SendMailBySmtp(
			string SmtpServer,
			string From, string To, string Cc, string Bcc, string ReplyTo,
			string Subject, string Body, MailPriority Priority,
			Attachment[] aAttachments, Encoding BodyEncoding)
		{
			//if (this.mSmtpServ.Status != ServiceControllerStatus.Running)
			//{
			//	this.mSmtpServ.Start();
			//}

			MailMessage MailMsg = new MailMessage(new MailAddress(From), new MailAddress(To));

			if (Cc.Length > 0) MailMsg.CC.Add(Cc);
			if (Bcc.Length > 0) MailMsg.Bcc.Add(Bcc);

			if (ReplyTo != "")
			{
#if DotNet35
				MailMsg.ReplyTo = new MailAddress(ReplyTo); //2.0
#else
				MailMsg.ReplyToList.Add(new MailAddress(ReplyTo));
#endif
			}

			MailMsg.SubjectEncoding = Encoding.UTF8; //UTF8이 아니면 To에서 한글이름이 사용되면 깨짐.
			MailMsg.Subject = Subject;
			MailMsg.IsBodyHtml = true;
			MailMsg.BodyEncoding = BodyEncoding;
			MailMsg.Body = Body;
			MailMsg.Priority = Priority;

			if (aAttachments != null)
			{
				foreach (Attachment att in aAttachments)
				{
					MailMsg.Attachments.Add(att);
				}
			}

			// Set the SmtpServer name. This can be any of the following depending on
			// your local security settings:
			// a) Local IP Address (assuming your local machine's SMTP server has the 
			// right to send messages through a local firewall (if present).
			// b) 127.0.0.1 the loopback of the local machine.
			// c) "smarthost" or the name or the IP address of the exchange server you 
			// utilize for messaging. This is usually what is needed if you are behind
			// a corporate firewall.
			// See the Readme file for more information.

			SmtpClient sc = new SmtpClient(SmtpServer);
			sc.Send(MailMsg);
		}
		/// <summary>
		/// SMTP 서버를 이용해 이메일을 전송함.
		/// </summary>
		/// <param name="From">보내는 메일 주소</param>
		/// <param name="To">받는 메일 주소</param>
		/// <param name="Subject">메일 제목</param>
		/// <param name="Body">메일 내용</param>
		/// <code>
		/// <![CDATA[
		/// CEmail em = new CEmail();
		/// em.SendMailBySmtp("운영자<doctorgu@naver.com>", "doctorgu@naver.com", "테스트 제목", "테스트 본문");
		/// ]]>
		/// </code>
		public void SendMailBySmtp(string From, string To, string Subject, string Body)
		{
			string SmtpServer = Environment.MachineName;
			string Cc = "", Bcc = "", ReplyTo = "";
			SendMailBySmtp(SmtpServer, From, To, Cc, Bcc, ReplyTo, Subject, Body, MailPriority.Normal, null, Encoding.UTF8);
		}
		public void SendMailBySmtp(string From, string To, string Subject, string Body, Encoding BodyEncoding)
		{
			string SmtpServer = Environment.MachineName;
			string Cc = "", Bcc = "", ReplyTo = "";
			SendMailBySmtp(SmtpServer, From, To, Cc, Bcc, ReplyTo, Subject, Body, MailPriority.Normal, null, BodyEncoding);
		}

		/// <summary>
		/// SMTP 서버를 이용해 이메일을 전송함.
		/// </summary>
		/// <param name="From">보내는 메일 주소</param>
		/// <param name="To">받는 메일 주소</param>
		/// <param name="ReplyTo">회신 메일 주소</param>
		/// <param name="Subject">메일 제목</param>
		/// <param name="Body">메일 내용</param>
		public void SendMailBySmtp(string From, string To, string ReplyTo, string Subject, string Body)
		{
			string SmtpServer = Environment.MachineName;
			string Cc = "", Bcc = "";
			SendMailBySmtp(SmtpServer, From, To, Cc, Bcc, ReplyTo, Subject, Body, MailPriority.Normal, null, Encoding.UTF8);
		}
		public void SendMailBySmtp(string From, string To, string ReplyTo, string Subject, string Body, Encoding BodyEncoding)
		{
			string SmtpServer = Environment.MachineName;
			string Cc = "", Bcc = "";
			SendMailBySmtp(SmtpServer, From, To, Cc, Bcc, ReplyTo, Subject, Body, MailPriority.Normal, null, BodyEncoding);
		}

		///// <summary>
		///// 현재 컴퓨터에 SMTP 서버가 설치되었는 지 여부를 리턴함.
		///// </summary>
		//public bool HasSmtpService
		//{
		//	get { return this.mHasSmtpService; }
		//}
	}
}
