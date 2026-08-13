# เอกสารออกแบบ (Design Documents)

เอกสารชุดนี้เขียนก่อนลงมือเขียนโค้ด เพื่อบันทึกว่าตัดสินใจอะไรและเพราะอะไร

| เอกสาร | เนื้อหา |
|---|---|
| [00 — โจทย์และการตีความ](design/00-requirements.md) | ข้อกำหนดจากข้อสอบ และจุดที่ต้องตีความเพิ่ม |
| [01 — สถาปัตยกรรม](design/01-architecture.md) | Clean Architecture 4 ชั้น และ MediatR pipeline |
| [02 — ฐานข้อมูล](design/02-database.md) | โครงสร้างตาราง เหตุผลที่เลือก SQLite และข้อมูล mockup |
| [03 — API](design/03-api.md) | endpoint, รูปแบบ request/response, สัญญาของ error |
| [04 — UI/UX](design/04-ui-ux.md) | ภาษาการออกแบบ, โครงหน้าจอ, การแปลง mockup เป็นของจริง |
| [05 — บันทึกการตัดสินใจ](design/05-decisions.md) | ADR ทุกข้อพร้อมทางเลือกที่ไม่ได้เลือกและเหตุผล |

สรุปรวมทั้งหมดอยู่ที่ [specs/2026-08-13-it03-approval-design.md](specs/2026-08-13-it03-approval-design.md)
