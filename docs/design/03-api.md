# 03 — API

Base URL ตอน dev: `http://localhost:5000` — Swagger UI อยู่ที่ `/swagger`

## Endpoint

| Method | Route | ใช้ที่ |
|---|---|---|
| GET | `/api/it03/documents` | ตาราง IT 03-1 |
| POST | `/api/it03/documents/approve` | Modal IT 03-2 |
| POST | `/api/it03/documents/reject` | Modal IT 03-3 |
| GET | `/api/it03/documents/{id}/logs` | หน้าประวัติการอนุมัติ |
| GET | `/api/it03/statuses` | หน้าข้อมูลสถานะเอกสาร |

## GET /api/it03/documents

```json
[
  {
    "id": 1,
    "documentName": "รายการที่ 1",
    "reason": "ขออนุมัติจัดซื้อวัสดุสำนักงาน ประจำเดือนสิงหาคม",
    "statusId": 1,
    "statusCode": "PENDING",
    "statusNameTh": "รออนุมัติ",
    "isPending": true,
    "createdAt": "2026-08-01T09:00:00",
    "updatedAt": "2026-08-01T09:00:00"
  }
]
```

ส่ง `isPending` มาให้ตรง ๆ เพื่อให้ frontend ไม่ต้องรู้ว่า `statusId == 1` หมายถึงอะไร
กฎว่าแถวไหนเลือกได้จึงมาจาก server ที่เดียว

ส่ง `statusCode` มาด้วยเพื่อให้ frontend ใช้ตั้งชื่อ CSS class ได้โดยไม่ต้องแปลข้อความไทย

## POST /api/it03/documents/approve และ /reject

```json
{ "documentIds": [1, 4, 7], "reason": "ตรวจสอบเอกสารครบถ้วน" }
```

ตอบกลับ

```json
{ "affectedCount": 3, "documentIds": [1, 4, 7], "statusNameTh": "อนุมัติ" }
```

### ทำไมรับเป็น array

UI เป็น bulk action (ดู [00-requirements](00-requirements.md)) ถ้า API รับทีละใบ frontend
จะต้องยิงหลายครั้งแล้วเจอปัญหาว่าใบที่ 3 ล้มเหลวแต่ใบที่ 1-2 สำเร็จไปแล้ว กลายเป็น
สถานะครึ่ง ๆ กลาง ๆ ที่ผู้ใช้ไม่ได้สั่ง

รับเป็น array แล้วทำใน transaction เดียวจึงได้ผลลัพธ์แบบ all-or-nothing

### ทำไมแยกสอง endpoint ไม่ใช่ `change-status` ตัวเดียว

สองเส้นทางอ่านแล้วรู้ทันทีว่าทำอะไร ตรงกับสองปุ่มและสอง modal ในข้อสอบ และ Swagger
อ่านง่ายกว่า ส่วนโค้ดที่ทำงานจริงยังเป็นตัวเดียวกัน (`DocumentDecisionExecutor`)
จึงไม่ได้แลกความซ้ำซ้อนมาแทน

## GET /api/it03/documents/{id}/logs

เรียงใหม่สุดขึ้นก่อน

```json
[
  {
    "id": 1,
    "documentId": 2,
    "fromStatusNameTh": "รออนุมัติ",
    "toStatusNameTh": "อนุมัติ",
    "reason": "ตรวจสอบเอกสารครบถ้วน อนุมัติตามระเบียบ",
    "actionBy": "somchai.k",
    "actionAt": "2026-08-03T09:00:00"
  }
]
```

## สัญญาของ error

ทุก error เป็น `ProblemDetails` (RFC 7807) รูปแบบเดียวกันหมด

| สถานการณ์ | HTTP | ตัวอย่าง |
|---|---|---|
| ไม่กรอกเหตุผล / ไม่เลือกรายการ | 400 | `{"title":"ข้อมูลไม่ถูกต้อง","status":400,"errors":{"Reason":["กรุณากรอกเหตุผล"]}}` |
| ไม่พบเอกสาร | 404 | `{"title":"ไม่พบข้อมูล","status":404,"detail":"ไม่พบเอกสารรหัส 999"}` |
| อนุมัติซ้ำ | 409 | `{"title":"ไม่สามารถดำเนินการได้","status":409,"detail":"เอกสาร 'รายการที่ 1' มีสถานะ 'อนุมัติ' อยู่แล้ว ไม่สามารถดำเนินการซ้ำได้"}` |
| ผิดพลาดภายใน | 500 | `{"title":"เกิดข้อผิดพลาดภายในระบบ","status":500}` |

500 ไม่ส่งรายละเอียดออกไป แต่ log ไว้ฝั่ง server

ฝั่ง Angular มี `describeError()` ตัวเดียวที่แปลง payload นี้เป็นข้อความบรรทัดเดียว
รวมกรณีต่อ API ไม่ติด (`status === 0`) ที่จะบอกให้ไปเปิด `dotnet run`

## ผลการทดสอบจริง

ยิงด้วย curl หลัง seed ใหม่

| กรณี | คาดหวัง | ได้ |
|---|---|---|
| approve `[1,4]` | 200, affected 2 | ผ่าน |
| approve `[1]` ซ้ำ | 409 | ผ่าน |
| reject `[2]` ที่อนุมัติแล้ว | 409 | ผ่าน |
| approve เหตุผลว่าง | 400 | ผ่าน |
| approve id 999 | 404 | ผ่าน |
| logs ของเอกสาร 1 | มี from/to ถูกต้อง | ผ่าน |
| ภาษาไทยผ่าน SQLite | อ่านกลับได้ครบ | ผ่าน |

## CORS

อนุญาตเฉพาะ `http://localhost:4200` ซึ่งเป็น Angular dev server
ตอน dev ยังมี proxy ที่ `web/proxy.conf.json` ส่ง `/api` ไปที่พอร์ต 5000
ทำให้ frontend เรียก path เดียวกับ production ได้
